using System.Threading.Tasks;

namespace SystemServices.Services
{
    public interface IReportGenerator
    {
        Task<Report> GenerateMountlySalesReport(string format);
        Task<Report> GenerateMountlyBalanceReport(string format);
    }
}
