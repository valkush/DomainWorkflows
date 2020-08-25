using System;
using System.Threading.Tasks;

using DomainWorkflows.Workflows;

namespace HelloWorld.Workflows
{
    [Singleton(RunAlways = true)]
    [Timeout("28 sec")]
    public class HelloWorldWorkflow : Workflow
    {
        protected override async Task OnStart()
        {
            Console.WriteLine("Hello World workflow started");
        }

        [Repeat("5 sec")]
        public void SayHello()
        {
            Console.WriteLine("Hello, world!");
        }

        protected async override Task OnTimeout()
        {
            Console.WriteLine("WORKFLOW TIMEOUT");
        }
    }
}
