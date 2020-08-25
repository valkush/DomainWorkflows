namespace Invoicing.Events
{
    public class InvoiceUpdated
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
    }
}
