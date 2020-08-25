using System.Threading.Tasks;

using Invoicing.Api;

namespace Invoicing.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task PayAsync(string refId, decimal amount)
        {
            Payment payment = await _paymentRepository.GetByRef(refId);
            if (payment == null)
                throw new PaymentNotFoundException();

            payment.Pay(amount);            

            _paymentRepository.Update(payment);
            await _paymentRepository.Save();
        }
    }
}