using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaxiCall.Domain;

namespace TaxiCall.Services
{
    public interface IDriverService
    {
        Task<Driver> GetDriver(int driverId);
    }
}
