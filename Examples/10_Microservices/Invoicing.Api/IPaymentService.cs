using System.Threading.Tasks;

namespace Invoicing.Api
{
    public interface IPaymentService
    {
        Task PayAsync(string refId, decimal amount);
    }
}
