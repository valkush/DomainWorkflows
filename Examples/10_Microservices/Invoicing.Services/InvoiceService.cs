using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using DomainWorkflows.Events;

using Invoicing.Api;
using Invoicing.Domain;
using Invoicing.Events;


namespace Invoicing.Services
{
    public sealed class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IEventSource _eventSource;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository invoiceRepository, IEventSource eventSource, ILogger<InvoiceService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _eventSource = eventSource;
            _logger = logger;
        }

        public async Task<int> Create(decimal total, string customerId, TimeSpan timeToPay)
        {
            DateTimeOffset dueDate = DateTimeOffset.Now.Add(timeToPay);
            Invoice invoice = new Invoice(Invoice.NewId, total, dueDate, customerId);

            _invoiceRepository.Add(invoice);
            await _invoiceRepository.Save();

            int invoiceId = invoice.Id;
            await _eventSource.Raise(new InvoiceCreated()
            {
                InvoiceId = invoiceId,
                TimeToPay = timeToPay,
                Amount = total
            });

            Log_InvoiceCreated(invoiceId, total);

            return invoiceId;
        }        

        public async Task Update(int invoiceId, decimal total)
        {
            Invoice invoice = await _invoiceRepository.Get(invoiceId);
            if (invoice == null)
                throw new InvoiceNotFoundException();

            invoice.Update(total);

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.Save();

            await _eventSource.Raise(new InvoiceUpdated()
            {
                InvoiceId = invoiceId,
                Amount = total
            });

            Log_InvoiceUpdated(invoiceId, total);
        }        

        public async Task<PaymentStatus> CheckPayment(int invoiceId)
        {
            Invoice invoice = await _invoiceRepository.Get(invoiceId);
            if (invoice == null)
                throw new InvoiceNotFoundException();

            return MapStatus(invoice.Status);
        }

        public async Task SetPaid(int invoiceId)
        {
            Invoice invoice = await _invoiceRepository.Get(invoiceId);
            if (invoice == null)
                throw new InvoiceNotFoundException();

            invoice.SetPaid();

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.Save();

            await _eventSource.Raise(new InvoicePaid()
            {
                InvoiceId = invoiceId
            });

            Log_InvoicePaid(invoiceId);
        }        

        public async Task SetOverdue(int invoiceId)
        {
            Invoice invoice = await _invoiceRepository.Get(invoiceId);
            if (invoice == null)
                throw new InvoiceNotFoundException();

            invoice.Overdue();

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.Save();

            Log_InvoiceOverdue(invoiceId);
        }        

        public async Task SetFaulted(int invoiceId)
        {
            Invoice invoice = await _invoiceRepository.Get(invoiceId);
            if (invoice == null)
                throw new InvoiceNotFoundException();

            invoice.SetFaulted();

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.Save();

            Log_InvoiceFaulted(invoiceId, invoice.Total);
        }        

        private PaymentStatus MapStatus(InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Pending: return PaymentStatus.NotPaid;
                case InvoiceStatus.Paid: return PaymentStatus.Paid;
                case InvoiceStatus.Overdue: return PaymentStatus.Overdue;
                case InvoiceStatus.Faulted: return PaymentStatus.Faulted;
                default: return PaymentStatus.NotPaid;
            }
        }


        #region Logging
        private void Log_InvoiceCreated(int invoiceId, decimal total)
        {
            _logger.LogInformation("Invoice Created [Id={InvoiceId}, Total={total}]", invoiceId, total);
        }

        private void Log_InvoiceUpdated(int invoiceId, decimal total)
        {
            _logger.LogInformation("Invoice Updated [Id={InvoiceId}, Total={total}]", invoiceId, total);
        }

        private void Log_InvoicePaid(int invoiceId)
        {
            _logger.LogInformation("Invoice Paid [Id={InvoiceId}]", invoiceId);
        }

        private void Log_InvoiceOverdue(int invoiceId)
        {
            _logger.LogWarning("Invoice Overdue [Id={InvoiceId}]", invoiceId);
        }

        private void Log_InvoiceFaulted(int invoiceId, decimal total)
        {
            _logger.LogError("Invoice Faulted [Id={InvoiceId}, Total={total}]", invoiceId, total);
        }
        #endregion
    }
}
