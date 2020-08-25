using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SystemServices.Services
{
    public class ReportGenerator : IReportGenerator
    {
        public async Task<Report> GenerateMountlyBalanceReport(string format)
        {
            return new Report();
        }

        public async Task<Report> GenerateMountlySalesReport(string format)
        {
            return new Report();
        }
    }
}
