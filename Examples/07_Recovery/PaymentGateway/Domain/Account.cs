using System;

namespace PaymentGateway.Domain
{
    public class Account
    {
        private decimal _authorizedAmount = 0.0M;
        
        public Account(string id, decimal balance = 0.0M)
        {
            this.Id = id;
            this.Balance = balance;
        }

        public string Id { get; }

        public decimal Balance { get; private set; }


        public void Autorize(decimal amount)
        {
            if (amount <= this.Balance - _authorizedAmount)
            {
                _authorizedAmount += amount;
            }
            else
            {
                throw new PaymentException("Not enough money on credit account");
            }
        }

        public void Withdraw(decimal amount, decimal authorizedAmount = 0.0M)
        {
            if (this.Balance < amount)
                throw new PaymentException("Not enough money on credit account");

            this.Balance -= amount;            

            if (authorizedAmount > 0.0M)
                this._authorizedAmount -= authorizedAmount;
        }

        public void Debit(decimal amount)
        {
            this.Balance += amount;
        }

        internal void Void(decimal amount, decimal authorizedAmount = 0.0M)
        {
            _authorizedAmount -= amount;

            if (authorizedAmount > 0.0M)
                this._authorizedAmount -= authorizedAmount;
        }
    }
}
