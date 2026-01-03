using EhjozProject.Domain.Models.Payment;

namespace EhjozProject.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByBookingIdAsync(int bookingId);
        Task<Payment> AddAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
    }
}