using System.Collections.Generic;
using System.Threading.Tasks;

namespace SystemServices.Services
{
    public interface IProgressService
    {
        Task<ICollection<int>> GetDailyProgressCustomers();
        Task<ICollection<int>> GetWeeklyProgressCustomers();
    }
}
