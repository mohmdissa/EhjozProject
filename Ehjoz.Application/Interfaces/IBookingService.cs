using EhjozProject.Domain.Models.Booking;

namespace EhjozProject.Application.Interfaces
{
    public interface IBookingService
    {
        // Get Methods
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<IEnumerable<Booking>> GetBookingsByStadiumIdAsync(int stadiumId);
        Task<IEnumerable<Booking>> GetBookingsByOwnerIdAsync(string ownerId);

        // Get Archived (Recycle Bin)
        Task<IEnumerable<Booking>> GetArchivedBookingsByOwnerIdAsync(string ownerId);

        // CRUD Operations
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);

        // Status Changes
        Task<bool> CancelBookingAsync(int id);
        Task<bool> ConfirmBookingAsync(int id);
        Task<bool> CompleteBookingAsync(int id);

        // Archive/Recycle Bin Operations
        Task<bool> ArchiveBookingAsync(int id);
        Task<bool> RestoreBookingAsync(int id);
        Task<bool> DeletePermanentlyAsync(int id);
        Task<bool> EmptyRecycleBinAsync(string ownerId);
    }
}