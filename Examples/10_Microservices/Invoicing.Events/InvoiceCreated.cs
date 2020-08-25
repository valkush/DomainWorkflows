using System;

namespace Invoicing.Events
{
    public sealed class InvoiceCreated
    {
        public int InvoiceId { get; set; }
        public TimeSpan TimeToPay { get; set; }
        public decimal Amount { get; set; }
    }
}
