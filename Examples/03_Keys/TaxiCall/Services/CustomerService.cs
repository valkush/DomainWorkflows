using System.Linq;
using System.Threading.Tasks;

using TaxiCall.Domain;

namespace TaxiCall.Services
{
    internal class CustomerService : ICustomerService
    {
        private Customer[] _customers = {
            new Customer() { Id = 1, Name="Customer1", Phone="cp1" },
            new Customer() { Id = 2, Name="Customer2", Phone="cp2" },
            new Customer() { Id = 3, Name="Customer3", Phone="cp3" },
        };

        public async Task<Customer> GetCustomer(int customerId)
        {
            return _customers.Where(c => c.Id == customerId).First();
        }
    }
}
