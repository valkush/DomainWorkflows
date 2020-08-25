# -----------------------------------------------------------------------------
System Services workflows perform system maintenance background
tasks. Usually they have daily, weekly and monthly schedules. For example, 
daily progress mailing or weekly data archiving. Maintenance workflows start
when host is starting because of Singleton(RunAlways = true) attribute. All 
schedules are defined with workflow parameters. Workflow parameters is 
logical variables to control workflow’s flow and defined in configuration file
or programmatically. Programmatically defined parameters can override parameters 
defined in configuration files.
Global Parameters are resolved to all workflows. Workflow Parameters are 
resolved only for defined workflows. Workflow Parameters overrides global 
parameters. 
We can define Workflow Parameters with special logical container named with “at”
symbol prefix and workflow name. For example, @Mailing defines parameters for 
Mailing workflow.
# -----------------------------------------------------------------------------
