using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DomainWorkflows.Hosting;
using DomainWorkflows.Utils.CommandProcessing;
using DomainWorkflows.DynamicContracts;
using DomainWorkflows.Events.Rebus;

using Invoicing.Api;
using Invoicing.Services;
using Invoicing.Domain;

using Rebus.Config;

namespace AppServicesHost
{
    class Program
    {
        private const string BusConnString = "amqp://localhost";

        static async Task Main(string[] args)
        {
            Console.Title = "APP SERVICES HOST";

            IHost workflowHost = new HostBuilder()             
             .ConfigureEnpoints(builder =>
             {
                 builder.EndpointName = "appservices";

                 builder.AddRebusEventChannel((options, channelId) =>
                     options.Logging(l => l.Console(Rebus.Logging.LogLevel.Error))
                            .Transport(t => t.UseRabbitMq(BusConnString, channelId)));

                 builder.Publish<IInvoiceService>();
                 builder.Publish<IPaymentService>(messagePattern: MessagePattern.Call);
                 builder.Publish<IPaymentGateway>();
             })
             .ConfigureLogging((logging) =>
             {
                 logging.AddConsole();
             })
             .ConfigureServices(services =>
             {
                 services.AddSingleton<IPaymentRepository, PaymentRepository>();
                 services.AddTransient<IPaymentGateway, PaymentGateway>();

                 services.AddSingleton<IInvoiceRepository, InvoiceRepositary>();
                 services.AddTransient<IInvoiceService, InvoiceService>();

                 services.AddTransient<IPaymentService, PaymentService>();
             })
             .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("AppServices host started.");
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
