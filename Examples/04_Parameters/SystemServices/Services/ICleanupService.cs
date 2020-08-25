using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SystemServices.Services
{
    public interface ICleanupService
    {
        Task<int> CleanupTempFiles();
    }
}
