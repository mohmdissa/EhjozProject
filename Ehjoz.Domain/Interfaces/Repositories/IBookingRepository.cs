using EhjozProject.Domain.Models.Booking;

namespace EhjozProject.Domain.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Booking>> GetByStadiumIdAsync(int stadiumId);
        Task<IEnumerable<Booking>> GetByOwnerIdAsync(string ownerId);
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
    }
}