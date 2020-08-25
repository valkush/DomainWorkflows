# -----------------------------------------------------------------------------
Classic Hello World workflows application prints “Hello, world!” greeting every
5 seconds by scheduler trigger. In 28 seconds, the workflow is shoot down by 
Workflow Timeout and all triggers become inactive relatively. Until now we 
get 5 “Hello, world!” greetings.
HelloWorld workflow starts automatically because of [Singleton(RunAlways = true)]
# -----------------------------------------------------------------------------
