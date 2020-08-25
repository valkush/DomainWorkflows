using Invoicing.Api;

namespace Invoicing.Services
{
    public class Payment
    {
        public string Id { get; internal set; }
        public decimal Amount { get; internal set; }
        public string RefId { get; internal set; }
        public bool IsPaid { get; internal set; }

        public void Pay(decimal amount)
        {
            if (amount != this.Amount)
                throw new PaymentException("Invalid payment amount");

            this.Amount = amount;
            this.IsPaid = true;
        }
    }
}
