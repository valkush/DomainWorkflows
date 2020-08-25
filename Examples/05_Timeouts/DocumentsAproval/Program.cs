using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DomainWorkflows.Hosting;
using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Utils.CommandProcessing;

using DocumentsAproval.Events;
using DocumentsAproval.Services;


namespace DocumentsAproval
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
                   services.AddTransient<IDocumentService, DocumentService>();
                   services.AddSingleton<IApprovalPolicy, ApprovalPolicy>();
                   services.AddTransient<IDocumentTaskService, DocumentTaskService>();
                   services.AddTransient<INotificationService, NotificationService>();
               })
               .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");
            Console.WriteLine();            

            ExpenseRequestCommandProcessor commandProcessor = new ExpenseRequestCommandProcessor(workflowHost);
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }
    }

    internal class ExpenseRequestCommandProcessor : CommandProcessor
    {
        private readonly IHost _workflowHost;

        public ExpenseRequestCommandProcessor(IHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        [Command("start", Description = "Start document approval")]
        public async Task StartApproval(int docId)
        {             
            await _workflowHost.SendEvent(new ExpenseApprovalRequested() { DocumentId = docId });
        }

        [Command("update", Description = "Upadte document")]
        public async Task UpdateDoc(int docId)
        {
            await _workflowHost.SendEvent(new ExpenseRequestUpdated() { DocumentId = docId });
        }

        [Command("review", Description = "Review document")]
        public async Task ReviewDoc(int docId)
        {
            await _workflowHost.SendEvent(new ExpenseRequestReviewed() { DocumentId = docId });
        }

        [Command("approve", Description = "Approve document")]
        public async Task ApproveDoc(int docId)
        {
            await _workflowHost.SendEvent(new ExpenseRequestApprove() { DocumentId = docId, Approved = true });
        }

        [Command("decline", Description = "Decline document")]
        public async Task DeclineDoc(int docId)
        {
            await _workflowHost.SendEvent(new ExpenseRequestApprove() { DocumentId = docId, Approved = false });
        }
    }
}
