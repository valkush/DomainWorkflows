using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IPaymentService
    {
        Task<string> Authorize(string fromAccountId, string toAccountId, decimal amount);
        Task Settle(string transactionId);
        Task Void(string transactionId);
        Task Refund(string transactionId);
    }
}
