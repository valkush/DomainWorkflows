using System;
using System.Threading.Tasks;

using DomainWorkflows.Workflows;

using Invoicing.Api;
using Invoicing.Events;

namespace Invoicing.Workflows
{    
    [Recovery(typeof(PaymentServiceUnavailableException))]
    public class InvoiceWorkflow : Workflow
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentGateway _paymentGateway;

        [Persist]
        private int _invoiceId;

        [Persist]
        private string _paymentId;

        [Persist]
        private int _slider = 0;

        public InvoiceWorkflow(IInvoiceService invoiceService, IPaymentGateway paymentGateway)
        {
            _invoiceService = invoiceService;
            _paymentGateway = paymentGateway;
        }

        [WorkflowStart]
        public async Task Handle(InvoiceCreated invoiceCreated)
        {
            base.AddParameter("TimeToPay", invoiceCreated.TimeToPay);

            _invoiceId = invoiceCreated.InvoiceId;            

            string refId = _invoiceId.ToString();
            _paymentId = await _paymentGateway.CreatePayment(refId, invoiceCreated.Amount);
        }

        public async Task Handle(InvoiceUpdated invoiceUpdated)
        {
            await _paymentGateway.UpdatePayment(_paymentId, invoiceUpdated.Amount);

            _slider++;
        }

        [Repeat("10 sec")]
        public async Task CheckPayment()
        {
            bool isPaid = await _paymentGateway.CheckStatus(_paymentId);
            if (isPaid)
            {
                await _invoiceService.SetPaid(_invoiceId);
                await Complete();
            }
        }        

        [Timeout("@TimeToPay", Sliding = "_slider")]
        public async Task InvoiceTimeout()
        {
            bool isPaid = await _paymentGateway.CheckStatus(_paymentId);
            if (isPaid == false)
            {
                await _invoiceService.SetOverdue(_invoiceId);
                await Timeout();
            }
            else
            {
                await _invoiceService.SetPaid(_invoiceId);
                await Complete();
            }           
        }

        protected async override Task OnFault(Exception exception)
        {
            await _invoiceService.SetFaulted(_invoiceId);            
        }
    }
}
