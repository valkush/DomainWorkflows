using System;

namespace Invoicing.Api
{    
    public class PaymentServiceUnavailableException : Exception
    {
        public PaymentServiceUnavailableException() : base ("Payment service temporarily unavailable")
        {
        }       
    }
}