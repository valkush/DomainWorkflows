using System;

namespace Invoicing.Api
{
    public class PaymentNotFoundException : Exception
    {
        public PaymentNotFoundException() : base("Payment is not found")
        {
        }
    }
}
