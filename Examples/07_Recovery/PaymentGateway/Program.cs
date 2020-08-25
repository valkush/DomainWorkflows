using System;
using System.Threading.Tasks;
using System.Globalization;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Workflows.Recovering;
using DomainWorkflows.Hosting;
using DomainWorkflows.Utils.CommandProcessing;

using PaymentGateway.Events;
using PaymentGateway.Services;


namespace PaymentGateway
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHost workflowHost = new HostBuilder()
             .UseWorkflows(builder =>
             {
                 // generate WorkflowFault events policy
                 builder.UseRecoveryBehavior(raiseFault: true);

                 // add recovery workflows
                 builder.AddRecovery();
             })
             .ConfigureLogging((logging) =>
             {
                 logging.AddConsole();
             })
             .ConfigureServices(services =>
             {
                 services.AddSingleton<PaymentStorage>();                 
                 services.AddTransient<IPaymentService, PaymentService>();
             })
             .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");
            Console.WriteLine();

            PaymentCommandProcessor commandProcessor = new PaymentCommandProcessor(workflowHost);
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }
    }


    internal class PaymentCommandProcessor : CommandProcessor
    {
        private readonly IHost _workflowHost;

        public PaymentCommandProcessor(IHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        [Command("pay", Description = "Make a payment (accId=1..10)")]
        public async Task MakePayment(string refId, string fromAccId, string toAccId, decimal amount)
        {
            await _workflowHost.SendEvent(new MakePayment()
            {
                RefId = refId,
                FromAccountId = fromAccId,
                ToAccountId = toAccId,
                Amount = amount
            });
        }

        [Command("cancel", Description = "Cancel payment")]
        public async Task CancelPayment(string refId)
        {
            await _workflowHost.SendEvent(new CancelPayment()
            {
                RefId = refId         
            });
        }

        [Command("enable-ps", Description = "Enable payment service")]
        public async Task EnablePaymentService()
        {
            using (var paymentStateExt = _workflowHost.GetExternalService<PaymentStorage>())
            {
                paymentStateExt.Value.Enabled = true;
                Console.WriteLine("PaymentService: ENABLED");
            }
        }

        [Command("disable-ps", Description = "Disable payment service")]
        public async Task DisablePaymentService()
        {
            using (var paymentStateExt = _workflowHost.GetExternalService<PaymentStorage>())
            {
                paymentStateExt.Value.Enabled = false;
                Console.WriteLine("PaymentService: DISABLED");
            }
        }
    }
}
