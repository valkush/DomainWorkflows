using System;

namespace PaymentGateway.Domain
{
    public class PaymentException : Exception
    {
        public PaymentException(string message) : base(message)
        {
        }
    }
}
