using System.Threading.Tasks;

namespace Messenger.Services
{
    public interface IEmailService
    {
        Task SendNewMessages(string userId);
    }
}
