using System.Collections.Generic;

using PaymentGateway.Domain;

namespace PaymentGateway.Services
{
    internal sealed class PaymentStorage
    {
        private const int AccountCount = 10;

        public PaymentStorage()
        {            
            List<Account> accounts = new List<Account>();
            for (int i = 0; i < AccountCount; i++)
            {
                string id = (i + 1).ToString();
                decimal balance = 10M * (i + 1);
                Account account = new Account(id, balance);
                accounts.Add(account); 
            }

            this.Accounts = accounts;
        }

        public bool Enabled { get; set; } = true;

        public List<Account> Accounts { get; }

        public Dictionary<string, Transaction> Transactions { get; } = new Dictionary<string, Transaction>();
    }
}
