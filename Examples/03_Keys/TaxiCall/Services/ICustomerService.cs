using System.Threading.Tasks;

using TaxiCall.Domain;

namespace TaxiCall.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomer(int customerId);
    }
}
