using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using DomainWorkflows.Workflows;

using TaxiCall.Domain;
using TaxiCall.Events;
using TaxiCall.Services;

namespace TaxiCall.Workflows
{
    [Key("CallId")]     // use CallId for all events in this workflow
    [Timeout("1 min")]
    public class TaxiCallWorkflow : Workflow
    {
        private readonly ICallService _callService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TaxiCallWorkflow> _logger;

        [Persist]
        private int _callId;

        public TaxiCallWorkflow(ICallService callService, INotificationService notificationService, ILogger<TaxiCallWorkflow> logger)
        {
            _callService = callService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [LateKey] // key data will be provided inside this handler - requires ExtractKey call
        [WorkflowStart]
        public async Task Handle(CallRequested request)
        {
            int callId = await _callService.CreateCall(request.CustomerId, request.AddressFrom, request.AddressTo);            

            // provide key data - must be called with LateKey attribute
            base.ExtractKey(callId);

            _callId = callId;

            // notify active drivers about new call
            await _notificationService.NotifyNewCall(callId);

            _logger.LogInformation("New call requested, CallId={CallId}", _callId);
        }

        // Exctace key from CallId field of pickup event defined by workflow [Key("CallId")] attribute
        public async Task Handle(CallPickedUp pickup)
        {
            await _callService.PickupCall(pickup.CallId, pickup.DriverId);


            await _notificationService.NotifyClosed(pickup.CallId);  // notify all other drivers the call is not active anymore


            await _notificationService.NotifyPickup(pickup.CallId); // send customer driver info to customer
            await _notificationService.ConfirmPickup(pickup.CallId); // send confirmation and customer details to driver

            await Complete();

            _logger.LogInformation("Call picked up, CallId={CallId}", _callId);
        }

        [Key("CallId")] // do nothing - can override workflow level Key attribute
        public async Task Handle(CallCanceled cancel)
        {
            await _callService.CancelCall(cancel.CallId);

            await _notificationService.NotifyClosed(cancel.CallId);

            _logger.LogInformation("Call canceled, CallId={CallId}", _callId);
        }

        protected async override Task OnTimeout()
        {                           
            await _callService.ExpireCall(_callId);

            await _notificationService.NotifyExpired(_callId);

            _logger.LogWarning("WORKFLOW TIMEOUT, CallId={CallId}", _callId);
        }

        // runs when an event comes after workflow has been closed - possible on events racing
        protected async override Task OnPostFinishEvent(object @event)
        {
            _logger.LogWarning("POST FINISH EVENT, EventType={EventType}", @event.GetType().Name);

            if (@event is CallPickedUp pickup)
            {
                await _notificationService.NotifyCompleted(_callId, Party.Driver);
            }
            else if (@event is CallCanceled callCanceled)
            {                
                await _notificationService.NotifyCompleted(_callId, Party.Customer);
            }
            else
            {
                // ignore others
                _logger.LogDebug("Post Finish Event ignored, EventType={EventType}", @event.GetType().Name);
            }
        }

        protected async override Task OnFault(Exception exception)
        {
            _logger.LogError(exception, "WORKFLOW FAULT.");

            await _notificationService.NotifyFault(_callId);        
        }        
    }
}
