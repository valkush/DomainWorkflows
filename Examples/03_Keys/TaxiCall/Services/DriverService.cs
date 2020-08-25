using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiCall.Domain;

namespace TaxiCall.Services
{
    internal class DriverService : IDriverService
    {
        private Driver[] _drivers = {
            new Driver() { Id = 1, Name = "Driver1", Phone = "dp1", CarInfo = "Car1" },
            new Driver() { Id = 2, Name = "Driver2", Phone = "dp2", CarInfo = "Car2" },
            new Driver() { Id = 3, Name = "Driver3", Phone = "dp3", CarInfo = "Car3" }
        };

        public async Task<Driver> GetDriver(int driverId)
        {
            return _drivers.Where(d => d.Id == driverId).FirstOrDefault();
        }
    }
}
