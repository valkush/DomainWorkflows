using System;

namespace PaymentGateway.Services
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message)
        {
        }
    }
}
