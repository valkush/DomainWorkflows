using System.Threading.Tasks;

namespace Invoicing.Services
{
    public interface IPaymentRepository
    {
        Task<Payment> Get(string patmentId);
        Task<Payment> GetByRef(string refId);
        void Add(Payment payment);
        void Update(Payment payment);
        Task Save();        
    }
}