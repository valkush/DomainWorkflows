using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Invoicing.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private static int _nextId = 0;

        private static readonly ConcurrentDictionary<string, Payment> _payments =
           new ConcurrentDictionary<string, Payment>();

        public async Task<Payment> Get(string patmentId)
        {
            if (_payments.TryGetValue(patmentId, out Payment payment))
                return payment;

            return null;
        }

        public async Task<Payment> GetByRef(string refId)
        {
            return _payments.Values
                .Where(p => p.RefId == refId)
                .FirstOrDefault();
        }

        public void Add(Payment payment)
        {
            string id = NextId();
            payment.Id = id;

            bool done = _payments.TryAdd(payment.Id, payment);
            if (!done)
                throw new InvalidOperationException("Concurency exception during new payment adding");
        }        

        public void Update(Payment payment)
        {   
            _payments[payment.Id] = payment;
        }

        public async Task Save()
        {
        }

        private string NextId()
        {
            int nextId = Interlocked.Increment(ref _nextId);
            return $"pid{nextId}";
        }
    }
}
