# -----------------------------------------------------------------------------
In order to test one or several workflows we use TestHost infrastructure. In
order to setup TestHost we need to add a testing workflow(s), dependencies and 
build a test plan for our case. These steps relate to the ARRANGE stage. To ACT
we just call host’s Run() method, which plays all workflow steps with timings.
All the steps are available in host’s Event Log. We can ASSERT test scenarios 
just by asserting events from Event Log and there timings.
To assert application services calls and related timings we redirect all service
interface calls to event queue. It means that any method call can be represented
as an event instance and queued. We use endpoint.Connect<T>() and 
endpoint.WireUp<T> method to hook up a calls interceptor and inject an interface
mock.
Test plan builder allows to raise defined events at defined time. These events 
are used to launch and play workflow’s logic.
Event Log extension methods allow us to find and assert events and their timings
in a convenient way.
# -----------------------------------------------------------------------------
