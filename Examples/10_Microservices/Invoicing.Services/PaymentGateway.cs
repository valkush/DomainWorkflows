using System;
using System.Threading.Tasks;

using Invoicing.Api;

namespace Invoicing.Services
{
    public sealed class PaymentGateway : IPaymentGateway
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentGateway(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }        

        public async Task<string> CreatePayment(string refId, decimal amount)
        {
            EmulateServiceUnavailable();

            Payment payment = new Payment()
            {                
                Amount = amount,
                RefId = refId,
                IsPaid = false
            };

            _paymentRepository.Add(payment);
            await _paymentRepository.Save();

            return payment.Id;
        }

        public async Task UpdatePayment(string paymentId, decimal amount)
        {
            EmulateServiceUnavailable();

            Payment payment = await _paymentRepository.Get(paymentId);
            if (payment == null)
                throw new PaymentNotFoundException();

            payment.Amount = amount;

            _paymentRepository.Update(payment);
            await _paymentRepository.Save();
        }         

        public async Task<bool> CheckStatus(string paymentId)
        {
            EmulateServiceUnavailable();

            Payment payment = await _paymentRepository.Get(paymentId);
            if (payment == null)
                throw new PaymentNotFoundException();

            return payment.IsPaid;
        }

        private static Random _rand = new Random();
        private void EmulateServiceUnavailable()
        {            
            int val = _rand.Next(1, 10);
            if (val == 5)
                throw new PaymentServiceUnavailableException();
        }
    }    
}
