using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Invoicing.Domain;

namespace Invoicing.Services
{
    public class InvoiceRepositary : IInvoiceRepository
    {
        private static int _nextId = 0;

        private ConcurrentDictionary<int, Invoice> _dic = new ConcurrentDictionary<int, Invoice>();

        public InvoiceRepositary()
        {            
        }

        public async Task<Invoice> Get(int invoiceId)
        {
            if (_dic.TryGetValue(invoiceId, out Invoice invoice))
                return invoice;

            return null;
        }

        public void Add(Invoice invoice)
        {
            int invoiceId = Interlocked.Increment(ref _nextId);
            invoice.Id = invoiceId;

            bool done = _dic.TryAdd(invoiceId, invoice);
            if (!done)
                throw new InvalidOperationException("Concurency exception during new invoice adding");
        }

        public void Update(Invoice invoice)
        {
            _dic[invoice.Id] = invoice;
        }

        public async Task Save()
        {            
        }
    }
}
