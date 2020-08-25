using System.Threading.Tasks;

using DomainWorkflows.Workflows;
using Microsoft.Extensions.Logging;

using SystemServices.Services;

namespace SystemServices.Workflows
{
    [Singleton(RunAlways = true)]
    public class MaintenanceWorkflow : Workflow
    {
        private readonly ICleanupService _cleanupService;
        private readonly IArchivingService _archivingService;
        private readonly IReportGenerator _reportGenerator;
        private readonly ILogger<MaintenanceWorkflow> _logger;

        public MaintenanceWorkflow(ICleanupService cleanupService, IArchivingService archivingService, IReportGenerator reportGenerator,
            ILogger<MaintenanceWorkflow> logger)
        {
            _cleanupService = cleanupService;
            _archivingService = archivingService;
            _reportGenerator = reportGenerator;
            _logger = logger;
        }

        // run every day at 5:20 AM
        //[Cron("0 20 5 ? * * *", State = "@Enable=true")]
        [Cron("@CleanupSchedule", State = "@Enable=true")]        
        public async Task CleanupTempFiles()
        {
            int count = await _cleanupService.CleanupTempFiles();
            _logger.LogInformation("{FileCount} tempoarary file(s) are cleaned up.", count);
        }

        // run every sun at 6:00 AM
        //[Cron("0 0 6 ? * SUN *", State = "@Enable=true")]
        [Cron("@ArchivingSchedule", State = "@Enable=true")]
        public async Task ArchiveWeekData()
        {
            await _archivingService.ArchiveWorkingData();
            _logger.LogInformation("Working data archived.");
        }

        // run last day of month at 7:30 AM
        //[Cron("0 30 7 L * ? *", State = "@Enable=true")]
        [Cron("@MontlyReportsSchedule", State = "@Enable=true")]
        public async Task GenerateMontlyReports()
        {
            string format = GetParameter("ReportFormat", "CSV");

            var balanceReport = await _reportGenerator.GenerateMountlyBalanceReport(format);
            _logger.LogInformation("Balance report is generated, Format={ReportFormat}", format);
            // use IReportStorage service to save the prepared report
            // use INotificationService to delivery the report to concerned users


            var salesReport = await _reportGenerator.GenerateMountlySalesReport(format);
            _logger.LogInformation("Sales report is generated, Format={ReportFormat}", format);
            // use IReportStorage service to save the prepared report
        }
    }
}
