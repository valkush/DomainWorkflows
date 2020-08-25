using System.Threading.Tasks;

using TaxiCall.Domain;

namespace TaxiCall.Services
{
    public interface ICallService
    {        
        Task<int> CreateCall(int customerId, string addressFrom, string addressTo);
        Task<Call> GetCall(int callId);

        Task CancelCall(int callId);
        Task PickupCall(int callId, int driverId);
        Task ExpireCall(int callId);
    }
}
