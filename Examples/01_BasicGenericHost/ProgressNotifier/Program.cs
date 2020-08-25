using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DomainWorkflows.Events;
using DomainWorkflows.Hosting;
using DomainWorkflows.Workflows.DependencyInjection;

using ProgressNotifier.Events;

namespace ProgressNotifier
{
    class Program
    {
        private static int _processId = 1;

        static async Task Main(string[] args)
        {
            // Build simple workflow host
            IHost workflowHost = new HostBuilder()
               .UseWorkflows()
               .ConfigureLogging((logging) =>
               {
                   logging.AddConsole();

                   logging.SetMinimumLevel(LogLevel.Warning);
                   logging.AddFilter("ProgressNotifier.Workflows", LogLevel.Information);
               })
               .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");

            // Raise ProgressRequest event in workflowHost and start initial workflow
            await workflowHost.RequestProgress(_processId++);

            Console.WriteLine("Press (Q) to exit or (S) to start new workflow instance");
            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S)
                {
                    await workflowHost.RequestProgress(_processId++); // start new workflow
                }
                else if (keyInfo.Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }        
        }        
    }

    static class HostExtensions
    {
        public static async Task RequestProgress(this IHost workflowHost, int processId)
        {
            using (ExternalService<IEventSource> eventSource = workflowHost.GetExternalService<IEventSource>())
            {
                ProgressRequested progressRequested = new ProgressRequested() { ProcessId = processId };
                await eventSource.Value.Raise(progressRequested);
            }
        }
    }
}
