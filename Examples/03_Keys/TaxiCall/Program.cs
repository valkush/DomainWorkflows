using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DomainWorkflows.Hosting;
using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Utils.CommandProcessing;

using TaxiCall.Services;
using TaxiCall.Events;


namespace TaxiCall
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHost workflowHost = new HostBuilder()
               .UseWorkflows()
               .ConfigureLogging((logging) =>
               {
                   logging.AddConsole();
               })
               .ConfigureServices(services =>
               {
                   services.AddSingleton<ICustomerService, CustomerService>();
                   services.AddSingleton<IDriverService, DriverService>();

                   services.AddSingleton<ICallService, CallService>();
                   services.AddTransient<INotificationService, NotificationService>();
               })
               .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");
            Console.WriteLine();            

            TaxiCallCommandProcessor commandProcessor = new TaxiCallCommandProcessor(workflowHost);
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }
    }

    internal class TaxiCallCommandProcessor : CommandProcessor
    {
        private const int DefaultCustomerId = 2; // choose form 1 to 3
        private const int DefaultDriverId = 3; // choose form 1 to 3

        private readonly IHost _workflowHost;

        public TaxiCallCommandProcessor(IHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        [Command("call", Description = "Call a taxi (as client)")]
        public async Task CallTaxi(string addrFrom, string addrTo, int customerId = DefaultCustomerId)
        {
            await _workflowHost.SendEvent(new CallRequested()
            {
                CustomerId = customerId,
                AddressFrom = addrFrom,
                AddressTo = addrTo
            });
        }

        [Command("pickup", Description = "Pickup (as driver)")]
        public async Task PickupCall(int callId, int driverId = DefaultDriverId)
        {
            await _workflowHost.SendEvent(new CallPickedUp()
            {
                DriverId = driverId,
                CallId = callId
            });
        }
    }
}
