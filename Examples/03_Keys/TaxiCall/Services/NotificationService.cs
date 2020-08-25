using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TaxiCall.Domain;

namespace TaxiCall.Services
{
    internal class NotificationService : INotificationService
    {
        private readonly ICallService _callService;
        private readonly IDriverService _driverService;
        private readonly ICustomerService _customerService;

        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ICallService callService, IDriverService driverService, ICustomerService customerService, 
            ILogger<NotificationService> logger)
        {
            _callService = callService;
            _driverService = driverService;
            _customerService = customerService;
            _logger = logger;
        }

        public async Task NotifyNewCall(int callId)
        {
            Call call = await _callService.GetCall(callId);

            // send to all active drivers
            _logger.LogInformation("CALL CREATED, CallId={CallId}, AddressFrom={From}, AddressTo={To}",
                callId,
                call.Info.AddressFrom,
                call.Info.AddressTo);
        }

        public async Task NotifyPickup(int callId)
        {
            Call call = await _callService.GetCall(callId);

            Driver driver = await _driverService.GetDriver(call.DriverId.Value);

            // send to customer
            _logger.LogInformation("TAXI FOUND, CallId={CallId}, Driver=[{DriverId},{DriverName},{DriverPhone},{Car}]",
                callId,
                driver.Id,
                driver.Name,
                driver.Phone,
                driver.CarInfo);
        }

        public async Task ConfirmPickup(int callId)
        {            
            Call call = await _callService.GetCall(callId);

            Customer customer = await _customerService.GetCustomer(call.DriverId.Value);

            // send customer info to driver
            _logger.LogInformation("CUSTOMER INFO, CallId={CallId}, Customer=[{CusmtomerId},{CustomerName},{CustomerPhone}]",
                callId,
                customer.Id,
                customer.Name,
                customer.Phone);
        }

        public async Task NotifyClosed(int callId)
        {
            // notify all drivers excluding currrent driver - call.DriverId            
            _logger.LogInformation("Call Closed, CallId={CallId}", callId);
        }        

        public async Task NotifyExpired(int callId)
        {
            // notify current customer - call.CustomerId
            _logger.LogInformation("Call Expired, CallId={CallId}", callId);
        }                

        public async Task NotifyCompleted(int callId, Party party)
        {
            // notify customer or driver
            _logger.LogInformation("Call has completed, CallId={CallId}, Party={Party}", callId, party);
        }

        public async Task NotifyFault(int callId)
        {
            
        }
    }
}
