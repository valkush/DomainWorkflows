using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Workflows.Parameters;

using SystemServices.Services;


namespace SystemServices
{
    class Program
    {
        static void Main(string[] args)
        {
            IHost workflowHost = new HostBuilder()
               .UseWorkflows((host, builder) =>
               {
                   // override config file parameters
                   builder.AddParameters(p =>
                   {
                       // add parameters form config file section
                       p.Configure(host.Configuration.GetSection("Parameters"));

                       // disable all mailing - override global Enable for Mailing workflow only
                       p.AddParameter("@Mailing", new { Enable = true });

                       // override montly report schedule - run every minute for testing
                       p.AddParameter("@Maintenance", new { MontlyReportsSchedule = "0 0/1 * * * ?" });
                   });
               })
               .ConfigureLogging((logging) =>
               {
                   logging.AddConsole();
               })
               .ConfigureServices(services =>
               {
                   services.AddTransient<IProgressService, ProgressService>();
                   services.AddTransient<INotificationService, NotificationService>();

                   services.AddTransient<ICleanupService, CleanupService>();
                   services.AddTransient<IArchivingService, ArchivingService>();
                   services.AddTransient<IReportGenerator, ReportGenerator>();
               })
               .ConfigureAppConfiguration(config =>
               {                   
                   config.AddJsonFile("workflowsconfig.json"); // add workflows config
               })
               .Build();

            workflowHost.Run();
        }
    }
}
