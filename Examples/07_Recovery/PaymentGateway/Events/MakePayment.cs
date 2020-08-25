namespace PaymentGateway.Events
{
    public class MakePayment
    {
        public string RefId { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
