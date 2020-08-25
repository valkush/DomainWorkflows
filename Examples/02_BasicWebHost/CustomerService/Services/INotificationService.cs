using System.Threading.Tasks;

namespace CustomerService.Services
{
    public interface INotificationService
    {
        Task NotifyCustomerQuestion(int questionId);
        Task NotifyCustomerAnswer(int questionId);
    }
}
