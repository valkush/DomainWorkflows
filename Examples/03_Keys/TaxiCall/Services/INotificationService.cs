using System.Threading.Tasks;

using TaxiCall.Domain;

namespace TaxiCall.Services
{
    public interface INotificationService
    {        
        Task NotifyNewCall(int callId);
        Task NotifyPickup(int callId);
        Task ConfirmPickup(int callId);
        Task NotifyClosed(int callId);
        Task NotifyExpired(int callId);
        Task NotifyCompleted(int callId, Party party);
        Task NotifyFault(int callId);
    }
}
