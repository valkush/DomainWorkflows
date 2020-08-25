using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using DomainWorkflows.Hosting;
using DomainWorkflows.Events;
using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Scheduler.Quartz;
using DomainWorkflows.Scheduler.Quartz.SqlServer;
using DomainWorkflows.Storage.Db;
using DomainWorkflows.Utils.CommandProcessing;

using WeatherWidget.Services;
using WeatherWidget.Events;

namespace WeatherWidget
{
    class Program
    {
        private const string QuartzDbStorageConnStr = @"server=.; initial catalog=SchedulerDb; integrated security=true";
        private const string WorkflowDbStorageConnString = @"Server=(localdb)\mssqllocaldb;Database=WorkflowDb;Trusted_Connection=True;";

        static async Task Main(string[] args)
        {
            IHost workflowHost = new HostBuilder()
              .UseWorkflows(builder =>
              {
                  // - External workflow storages requires a license purchase
                  // - Else we will get License exception when uncomment these settings


                  //builder.AddQuartzScheduler().UseSqlServerStorage(QuartzDbStorageConnStr); // CONFIG DB SCHEDULER STORAGE

                  //builder.AddConcurrentDbWorkflowStorage(options => options                 // CONFIG DB WORKFLOW STOGARE
                  //    .UseSqlServer(WorkflowDbStorageConnString))
                  //    .UseConcurrencyBehavior(lockTimeout: TimeSpan.FromSeconds(4));
              })
              .ConfigureLogging((logging) =>
              {
                  logging.AddConsole();
              })
              .ConfigureServices(services =>
              {
                  services.AddSingleton<IWeatherService, WeatherService>();
              })
              .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");
            Console.WriteLine();

            WeatherCommandProcessor commandProcessor = new WeatherCommandProcessor(workflowHost);
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }        
    }


    internal class WeatherCommandProcessor : CommandProcessor
    {
        private readonly IHost _workflowHost;

        public WeatherCommandProcessor(IHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        [Command("check", Description = "Get current weather in a region (1 to 5)")]
        public async Task MakePayment(string regionId = "1")
        {
            var response = (WeatherResponse)await _workflowHost.Call(new WeatherRequest() { RegionId = regionId });
            await ShowCurrentWeather(response);            
        }

        private static async Task ShowCurrentWeather(WeatherResponse response)
        {
            await Task.Delay(100);

            Console.WriteLine("CURRENT WEATHER FOR [{0}]: +{1}, {2}km/h, {3}%",
                response.RegionId,
                response.Temperature,
                response.Wind,
                response.Humidity);
        }
    }
}
