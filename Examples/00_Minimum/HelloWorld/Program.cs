using Microsoft.Extensions.Hosting;

using DomainWorkflows.Workflows.DependencyInjection;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            IHost workflowHost = new HostBuilder()
               .UseWorkflows()
               .Build();

            workflowHost.Run();
        }
    }
}
