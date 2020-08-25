using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DomainWorkflows.Hosting;
using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Utils.CommandProcessing;
using DomainWorkflows.Events.Rebus;
using DomainWorkflows.Workflows.Recovering;

using Invoicing.Api;

using Rebus.Config;

namespace WorkflowHost
{
    class Program
    {
        private const string BusConnString = "amqp://localhost";

        static async Task Main(string[] args)
        {
            Invoicing.Workflows.InvoiceWorkflow explicitRef = null;

            Console.Title = "WORKFLOW HOST";

            IHost workflowHost = new HostBuilder()
             .UseWorkflows(builder =>
             {
                 builder.AddRecovery(policies => 
                       policies.Add(
                           workflowId: "Invoice",
                           recoveryWorkflowId: "Recovery",
                           parameters: new { MaxTryCount = 3, RetryInterval = "25 sec", CompleteInterval = "2 min" })
                        ); 
             })
             .ConfigureEnpoints(builder =>
             {
                 builder.EndpointName = "worflows";

                 builder.AddRebusEventChannel((options, channelId) =>
                     options.Logging(l => l.Console(Rebus.Logging.LogLevel.Error))
                            .Transport(t => t.UseRabbitMq(BusConnString, channelId)));

                 builder.Connect<IInvoiceService>();
                 builder.Connect<IPaymentGateway>();
             })
             .ConfigureLogging((logging) =>
             {
                 logging.AddConsole();

                 //logging.SetMinimumLevel(LogLevel.Warning);

                 logging.AddFilter("DomainWorkflows.Workflows.Recovering", LogLevel.Debug);
                 //logging.AddFilter("DomainWorkflows.Workflows", LogLevel.Debug);
                 //logging.AddFilter("DomainWorkflows.Workflows.WorkflowInvoker", LogLevel.Trace);
             })             
             .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");
            Console.WriteLine();            

            CommandProcessor commandProcessor = new CommandProcessor();
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }
    }
}
