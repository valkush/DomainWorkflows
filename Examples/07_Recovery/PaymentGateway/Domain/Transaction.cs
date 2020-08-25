using DomainWorkflows.Utils;

namespace PaymentGateway.Domain
{
    public sealed class Transaction
    {
        public string Id { get; }

        public TransactionStatus Status { get; private set; }

        public Account FromAccount { get; }
        public Account ToAccount { get; }
        public decimal Amount { get; }

        private decimal _autorizedAmount = 0.0M;

        public Transaction(Account fromAccount, Account toAccount, decimal amount)
        {
            if (fromAccount == null)
                throw new AccountNotFoundException("Credit account is not found");

            if (toAccount == null)
                throw new AccountNotFoundException("Debit account is not found");

            this.FromAccount = fromAccount;
            this.ToAccount = toAccount;

            this.Amount = amount;

            this.Id = SysUtils.GetRandomString(6);            
        }        

        public void Autorize()
        {
            this.FromAccount.Autorize(this.Amount);

            this.Status = TransactionStatus.Autorized;
            _autorizedAmount = this.Amount;
        }

        public void Settle()
        {
            this.FromAccount.Withdraw(this.Amount, _autorizedAmount);
            this.ToAccount.Debit(this.Amount);

            this.Status = TransactionStatus.Settled;
        }

        public void Void()
        {
            this.FromAccount.Void(this.Amount);

            this.Status = TransactionStatus.Voided;
        }
    }
}
