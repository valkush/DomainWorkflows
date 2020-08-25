using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SystemServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendDailyProgress(int customerId)
        {
            _logger.LogInformation("DailyProgress sent, CustomerId={CustomerId}", customerId);
        }

        public async Task SendWeeklyProgress(int customerId)
        {
            _logger.LogInformation("WeeklyProgress sent, CustomerId={CustomerId}", customerId);
        }
    }
}
