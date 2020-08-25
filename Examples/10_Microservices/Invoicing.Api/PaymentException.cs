using System;

namespace Invoicing.Api
{
    public class PaymentException : Exception
    {
        public PaymentException(string message) : base(message)
        {
        }
    }
}
