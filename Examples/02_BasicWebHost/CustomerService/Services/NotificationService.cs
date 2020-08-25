using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace CustomerService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task NotifyCustomerQuestion(int questionId)
        {
            _logger.LogWarning("WE RECEIVED YOUR QUESTION");
        }

        public async Task NotifyCustomerAnswer(int questionId)
        {
            _logger.LogWarning("YOUR QUESTION ANSWERED: answer");
        }        
    }
}
