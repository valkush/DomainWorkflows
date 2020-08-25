using System;
using System.Threading.Tasks;

namespace Reviewing.Services
{
    internal class NotificationService : INotificationService
    {
        public async Task NotifyReviewAssigned(int userId, int articleId)
        {
            Console.WriteLine("HELLO, YOU ARE ASSIGNED AS REVIWER FOR ARTICLE={0}", articleId);
        }
    }
}
