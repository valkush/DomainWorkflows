using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using DomainWorkflows.Workflows;

using PaymentGateway.Domain;
using PaymentGateway.Events;
using PaymentGateway.Services;

namespace PaymentGateway.Workflows
{
    //[Recovery(typeof(ServiceUnavailableException))]
    [Key("RefId")]
    public class PaymentWorkflow : Workflow
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentWorkflow> _logger;

        [Persist]
        private string _refId;
        [Persist]
        private string _transactionId = null;
        [Persist]
        private bool _settled = false;

        public PaymentWorkflow(IPaymentService paymentService, ILogger<PaymentWorkflow> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [WorkflowStart]        
        public async Task Handle(MakePayment payment)
        {
            _refId = payment.RefId;

            _transactionId = await _paymentService.Authorize(payment.FromAccountId, payment.ToAccountId, payment.Amount);

            _logger.LogInformation("Payment was authorized, RefId={RefId}, Amount={Amount}", payment.RefId, payment.Amount);
        }

        [Timeout("1 min", State = "_settled = false")]
        public async Task SettlePayment()
        {
            await _paymentService.Settle(_transactionId);
            _settled = true;

            _logger.LogInformation("Payment is settled, RefId={RefId}", _refId);
        }

        [Timeout("3 min", State = "_settled = true")]
        public async Task CompletePayment()
        {
            await this.Complete();
        }

        public async Task Handle(CancelPayment payment)
        {
            if (!_settled)
            {
                await _paymentService.Void(_transactionId);

                _logger.LogInformation("Payment is voided, RefId={RefId}", _refId);
            }
            else
            {
                await _paymentService.Refund(_transactionId);

                _logger.LogWarning("Payment is refunded, RefId={RefId}", _refId);
            }

            await this.Complete();
        }

        protected async override Task<FaultAction> OnPreFault(Exception exception)
        {
            // mark the exception as recoverable
            // this is the same as [Recovery(typeof(ServiceUnavailableException))]
            if (exception is ServiceUnavailableException)
                return FaultAction.Recovery;

            return await base.OnPreFault(exception);
        }

        protected async override Task OnFault(Exception exception)
        {
            if (exception is PaymentException pe)
            {
                // handle payment faults in a special way
                _logger.LogError("Payment error - {ErrorMessage}, RefId={RefId}",
                    pe.Message, _refId);
            }
        }
    }
}
