using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TaxiCall.Domain;

namespace TaxiCall.Services
{
    internal class CallService : ICallService
    {
        private static int NextId = 1;

        private List<Call> _calls = new List<Call>();

        public async Task<int> CreateCall(int customerId, string addressFrom, string addressTo)
        {
            Call call = new Call()
            {
                Id = NextId++,
                CustomerId = customerId,

                Info = new CallInfo()
                {
                    AddressFrom = addressFrom,
                    AddressTo = addressTo
                },

                Status = CallStatus.Pending
            };

            _calls.Add(call);

            return call.Id;
        }

        public async Task<Call> GetCall(int callId)
        {
            Call call = _calls.Where(c => c.Id == callId).FirstOrDefault();
            if (call == null)
                throw new InvalidOperationException("Call is not exists");

            return call;
        }

        public async Task CancelCall(int callId)
        {
            Call call = _calls.Where(c => c.Id == callId).First();
            call.Status = CallStatus.Canceled;
        }

        public async Task PickupCall(int callId, int driverId)
        {
            Call call = _calls.Where(c => c.Id == callId).First();

            call.DriverId = driverId;
            call.Status = CallStatus.Closed;
        }

        public async Task ExpireCall(int callId)
        {
            Call call = _calls.Where(c => c.Id == callId).First();
            call.Status = CallStatus.Expired;
        }        
    }
}
