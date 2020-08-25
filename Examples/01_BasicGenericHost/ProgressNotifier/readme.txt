# -----------------------------------------------------------------------------
Simple setup to lunch workflows can be done by calling UseWorkflows() extension
on Generic Host Builder. After .net core generic host has started, we are 
completely ready to run any workflows. All workflows and their settings will be
scan folded automatically using defined conventions.
Now to start or update any workflow we need only raise related event in the
Workflows host. We use IEventSource interface to raise all events. If we need 
to raise external event (relative to workflow host) we use ExternalService
pattern. IEventSource can be injected anywhere inside a workflow host and be used
for events raising.
The simple example demonstrates two-way events exchange. The client sends 
ProgresRequest to start workflow and then receive ProgressChanged events until
scheduler trigger off by State condition. Then workflows will be finished by
workflow timeout.
# -----------------------------------------------------------------------------

### TESTING STEPS ###

# first workflow is started automatically on host starting

# start a new progress
cmd > s

# start one more progress and watch couning
cmd > s

# wait all workflows are done and/or watch progress

cmd > q

# workflow host is stopped and workflows stop handle any events  
# HAVE FUN! )




