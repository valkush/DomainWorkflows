using System;
using System.Threading.Tasks;

namespace Invoicing.Api
{
    public interface IInvoiceService
    {
        Task<int> Create(decimal total, string customerId, TimeSpan timeToPay);
        Task Update(int invoiceId, decimal total);
        Task<PaymentStatus> CheckPayment(int invoiceId);

        Task SetOverdue(int invoiceId);
        Task SetPaid(int invoiceId);
        Task SetFaulted(int invoiceId);
    }
}
