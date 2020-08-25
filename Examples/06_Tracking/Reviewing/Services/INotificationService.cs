using System.Threading.Tasks;

namespace Reviewing.Services
{
    public interface INotificationService
    {
        Task NotifyReviewAssigned(int userId, int articalId);
    }
}
