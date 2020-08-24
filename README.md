# DomainWorkflows

DomainWorkflows is powerful workflow engine with simple and clear programming model. It is cross-platform and supports netstandard 2.0, .NET Core 2.2 and .NET Framework 4.6.1 or later.

*This repository contains examples of DomainWorkflows use in different real-world scenarios.*

## Basics

DomainWorkflows is designed to add long-running and scheduling capabilities to core domain layers. The basic DomainWorkflows concept is isolating developers from all complexity of infrastructure plumbing and allow to  focus on solving business problems. At the same time you can customize or extend almost any aspect of DomainWorkflows behavior using customization API. Typically, DomainWorkflows components work at the same level as App Services and orchestrate App Services and/or manage domain objects directly.

In DomainWorkflows model every workflow component is focused, testable and persistent unit. Every workflow is tightly integrated into the dependency injection container and have direct access to all DI services. But actually, every workflow is also a distributed, event-driven and self-recoverable actor component which implements best practices of asynchronous message-based systems. DomainWorkflows tries to unit best of both simplicity of synchronous programming model and the power of asynchronous distributed systems.

## Features

Current version of DomainWorkflows provides next basic features:
- Generic Host integration
- flexible scheduling
- expressions
- custom pipelines
- dynamic contracts
- extensive logging
- persistence
- recovering
- parameters
- tracking
- testing
- microservices

## Installing

You can install DomainWorkflows with NuGet:

     Install-Package DomainWorkflows 

Or via the .NET Core command line interface:

    dotnet add package DomainWorkflows 

All required dependencies will be downloaded and installed automatically.

## Setup

Install the package via NuGet first

	Install-Package DomainWorkflows

Add DomainWorkflows to .Net Generic Host

``` CSharp
IHost workflowHost = new HostBuilder()
    .UseWorkflows()
    .Build();

workflowHost.Run();
``` 

Or if you use ASP.NET Core you can add DomainWorkflows to Web Host in `ConfigureServices` startup method

``` CSharp
public void ConfigureServices(IServiceCollection services)
{
    // configure other services
	
    services.AddWorkflows();
}
```

## Usage

By default DomainWorkflows uses conventional workflow's locations. Create new Workflows folder for your first workflow. You can also place your workflows in a separate assembly which name ends with '.Workflows'. 
Add new workflow class `HelloWorldWorkflow` to created Workflows folder and copy this code

``` Csharp    
[Singleton(RunAlways = true)]
[Timeout("32 sec")]
public class HelloWorldWorkflow : Workflow
{
  [Persist]
  private int _count = 1;

  protected override async Task OnStart()
  {
    Console.WriteLine("Hello World workflow started");
  }

  [Repeat("5 sec", State = "_count <= 5")]
  public void SayHello()
  {
    Console.WriteLine($"Hello, world! - {_count}");

    _count++;
  }

  protected async override Task OnTimeout()
  {
    Console.WriteLine("WORKFLOW TIMEOUT");
  }
}
```

The `HelloWorldWorkflow` will print Hello World greeting five times. `SayHello` method is called every 5 seconds because we attached `Repeat` schedule attribute to this method. As soon as State condition of `Repeat` schedule become false, the schedule will be canceled and the method will not be called anymore.

Workflow `Timeout` attribute is set to 32 seconds. After this period the workflow will transit in *Timeout State*. It means all workflow schedulers will be canceled and the workflow will not able to  handle any requests anymore.

We added `Singleton(RunAlways = true)` attribute to start the workflow when host has started so we needn't wait for any workflow activation events.

`OnStart` and `OnTimeout` methods are optional in this example and used only to trace the workflow state.
