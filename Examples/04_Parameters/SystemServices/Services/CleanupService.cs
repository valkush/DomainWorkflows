using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SystemServices.Services
{
    public class CleanupService : ICleanupService
    {
        public async Task<int> CleanupTempFiles()
        {
            // clean up logic
            return 326;
        }
    }
}
