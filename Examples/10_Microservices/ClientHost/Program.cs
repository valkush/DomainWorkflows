using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DomainWorkflows.Hosting;
using DomainWorkflows.Utils.CommandProcessing;
using DomainWorkflows.Events.Rebus;
using DomainWorkflows.DynamicContracts;

using Rebus.Config;

using Invoicing.Api;

namespace ClientHost
{
    class Program
    {        
        private const string BusConnString = "amqp://localhost";

        static async Task Main(string[] args)
        {            
            Console.Title = "CLIENT HOST";

            IHost workflowHost = new HostBuilder()
             //.UseWorkflows()
             //{
                 //builder.AddRecovery(policies =>
                 //      policies.Add(
                 //          workflowId: "Invoice",
                 //          recoveryWorkflowId: "Recovery",
                 //          parameters: new { MaxTryCount = 3, RetryInterval = "25 sec", CompleteInterval = "2 min" })
                 //       );
             //})
             .ConfigureEnpoints(builder =>
             {
                 builder.EndpointName = "client";
                 //builder.HostId = "1";

                 builder.AddRebusEventChannel((options, channelId) =>
                     options.Logging(l => l.Console(Rebus.Logging.LogLevel.Error))
                            .Transport(t => t.UseRabbitMq(BusConnString, channelId)));

                 builder.Connect<IInvoiceService>();
                 builder.Connect<IPaymentService>(messagePattern:MessagePattern.Call);
             })
             .ConfigureLogging((logging) =>
             {
                 logging.AddConsole();
             })
             //.ConfigureServices(services =>
             //{
             //    Invoicing.Workflows.InvoiceWorkflow explicitRef = null;

             //    services.AddSingleton<IPaymentRepository, PaymentRepository>();
             //    services.AddTransient<IPaymentGateway, PaymentGateway>();

             //    services.AddSingleton<IInvoiceRepository, InvoiceRepositary>();
             //    services.AddTransient<IInvoiceService, InvoiceService>();

             //    services.AddTransient<IPaymentService, PaymentService>();

             //})             
             .Build();            

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow client started.");
            Console.WriteLine();            

            InvoicingCommandProcessor commandProcessor = new InvoicingCommandProcessor(workflowHost);
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }
    }

    internal class InvoicingCommandProcessor : CommandProcessor
    {
        private readonly IHost _host;

        public InvoicingCommandProcessor(IHost workflowHost)
        {
            _host = workflowHost;
        }

        [Command("create", Description = "Create new invoice")]
        public async Task CreateInvoice(decimal total, int minsToPay, string customerId = "1")
        {
            await _host.UseServiceAsync<IInvoiceService>(async invoiceService =>
                {
                    TimeSpan timeToPay = TimeSpan.FromMinutes(minsToPay);
                    int invoiceId = await invoiceService.Create(total, customerId, timeToPay);

                    Console.WriteLine($"  Invoice created [Id={invoiceId}, Total={total}]");
                });            
        }

        [Command("update", Description = "Update invoice total")]
        public async Task UpdateInvoice(int invoiceId, decimal total)
        {
            await _host.UseServiceAsync<IInvoiceService>(async invoiceService =>
                {
                    await invoiceService.Update(invoiceId, total);

                    Console.WriteLine($"  Invoice updated [Id={invoiceId}, Total={total}]");
                });                
        }

        [Command("pay", Description = "Pay invoice")]
        public async Task Pay([Arg("InvoiceId")]string refId, decimal total)
        {
            await _host.UseServiceAsync<IPaymentService>(async paymentService =>
            {
                await paymentService.PayAsync(refId, total);

                Console.WriteLine($"  Payment Success [RefId={refId}]");
            });
        }

        [Command("check", Description = "Check invoice payment")]
        public async Task Check(int invoiceId)
        {
            await _host.UseServiceAsync<IInvoiceService>(async invoiceService =>
            {
                PaymentStatus status = await invoiceService.CheckPayment(invoiceId);
                Console.WriteLine($"  Invoice [Id={invoiceId}]: {status}");
            });
        }
    }
}
