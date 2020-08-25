using System.Threading.Tasks;

namespace SystemServices.Services
{
    public interface INotificationService
    {
        Task SendDailyProgress(int customerId);
        Task SendWeeklyProgress(int customerId);
    }
}
