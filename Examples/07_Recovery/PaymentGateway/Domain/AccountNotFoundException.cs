namespace PaymentGateway.Domain
{
    public class AccountNotFoundException : PaymentException
    {
        public AccountNotFoundException(string message) : base(message)
        {
        }
    }
}
