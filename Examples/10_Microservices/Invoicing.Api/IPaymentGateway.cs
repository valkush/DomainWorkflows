using System.Threading.Tasks;

namespace Invoicing.Api
{
    public interface IPaymentGateway
    {
        Task<string> CreatePayment(string refId, decimal amount);
        Task UpdatePayment(string paymentId, decimal amount);         
        Task<bool> CheckStatus(string paymentId);
    }
}
