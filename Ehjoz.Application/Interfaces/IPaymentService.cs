using EhjozProject.Domain.Models.Payment;

namespace EhjozProject.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<Payment?> GetPaymentByBookingIdAsync(int bookingId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<bool> MarkAsPaidAsync(int paymentId);
    }
}