using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Payment;

namespace EhjozProject.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepository.GetByIdAsync(id);
        }

        public async Task<Payment?> GetPaymentByBookingIdAsync(int bookingId)
        {
            return await _paymentRepository.GetByBookingIdAsync(bookingId);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            payment.PaymentDate = DateTime.Now;
            payment.IsPaid = false;
            return await _paymentRepository.AddAsync(payment);
        }

        public async Task<bool> MarkAsPaidAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) return false;

            payment.IsPaid = true;
            payment.PaymentDate = DateTime.Now;
            await _paymentRepository.UpdateAsync(payment);
            return true;
        }
    }
}