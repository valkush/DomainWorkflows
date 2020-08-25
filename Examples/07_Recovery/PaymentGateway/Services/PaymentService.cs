using System.Linq;
using System.Threading.Tasks;

using PaymentGateway.Domain;

namespace PaymentGateway.Services
{
    internal sealed class PaymentService : IPaymentService
    {
        private readonly PaymentStorage _storage;

        public PaymentService(PaymentStorage storage)
        {
            _storage = storage;
        }

        public async Task<string> Authorize(string fromAccountId, string toAccountId, decimal amount)
        {
            CheckServiceEnabled();

            Account fromAccount = _storage.Accounts.Where(acc => acc.Id == fromAccountId).FirstOrDefault();
            Account toAccount = _storage.Accounts.Where(acc => acc.Id == toAccountId).FirstOrDefault();

            Transaction transaction = new Transaction(fromAccount, toAccount, amount);
            transaction.Autorize();

            _storage.Transactions.Add(transaction.Id, transaction);

            return transaction.Id;
        }

        public async Task Settle(string transactionId)
        {
            CheckServiceEnabled();

            Transaction transaction = null;
            if (!_storage.Transactions.TryGetValue(transactionId, out transaction))
                throw new PaymentException("Transaction is not found");

            transaction.Settle();
        }

        public async Task Void(string transactionId)
        {
            CheckServiceEnabled();

            Transaction transaction = null;
            if (!_storage.Transactions.TryGetValue(transactionId, out transaction))
                throw new PaymentException("Transaction is not found");

            transaction.Void();
        }

        public async Task Refund(string transactionId)
        {
            CheckServiceEnabled();

            Transaction transaction = null;
            if (!_storage.Transactions.TryGetValue(transactionId, out transaction))
                throw new PaymentException("Transaction is not found");

            Transaction refundTransaction = new Transaction(transaction.ToAccount, transaction.FromAccount, transaction.Amount);
            refundTransaction.Settle();
        }

        private void CheckServiceEnabled()
        {
            if (_storage.Enabled == false)
                throw new ServiceUnavailableException("Payment service");
        }
    }
}
