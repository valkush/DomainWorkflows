using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SystemServices.Services
{
    public class ProgressService : IProgressService
    {
        public async Task<ICollection<int>> GetDailyProgressCustomers()
        {
            return new int[] { 120, 223, 73,12 };
        }

        public async Task<ICollection<int>> GetWeeklyProgressCustomers()
        {
            return new int[] { 120, 27, 22, 12, 15 , 10, 78, 22 };
        }
        
    }
}
