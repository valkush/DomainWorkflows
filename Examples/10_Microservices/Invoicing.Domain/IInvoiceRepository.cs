using System.Threading.Tasks;

namespace Invoicing.Domain
{
    public interface IInvoiceRepository
    {
        Task<Invoice> Get(int invoiceId);
        void Add(Invoice invoice);
        void Update(Invoice invoice);
        Task Save();
    }
}
