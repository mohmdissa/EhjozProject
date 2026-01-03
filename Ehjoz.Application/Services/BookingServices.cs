using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Booking;

namespace EhjozProject.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;

        public BookingService(IBookingRepository bookingRepository, ITimeSlotRepository timeSlotRepository)
        {
            _bookingRepository = bookingRepository;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync() ?? new List<Booking>();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _bookingRepository.GetByUserIdAsync(userId) ?? new List<Booking>();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStadiumIdAsync(int stadiumId)
        {
            return await _bookingRepository.GetByStadiumIdAsync(stadiumId) ?? new List<Booking>();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByOwnerIdAsync(string ownerId)
        {
            return await _bookingRepository.GetByOwnerIdAsync(ownerId) ?? new List<Booking>();
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            // Set default status
            booking.Status = "Pending";

            // Mark time slot as booked
            var timeSlot = await _timeSlotRepository.GetByIdAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = false;
                await _timeSlotRepository.UpdateAsync(timeSlot);
            }

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            return await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = "Cancelled";
            await _bookingRepository.UpdateAsync(booking);

            // Mark time slot as available again
            var timeSlot = await _timeSlotRepository.GetByIdAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = true;
                await _timeSlotRepository.UpdateAsync(timeSlot);
            }

            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = "Confirmed";
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> CompleteBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = "Completed";
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<IEnumerable<Booking>> GetArchivedBookingsByOwnerIdAsync(string ownerId)
        {
            var bookings = await _bookingRepository.GetByOwnerIdAsync(ownerId);

            if (bookings == null)
                return new List<Booking>();

            return bookings.Where(b => b.IsArchived == true).OrderByDescending(b => b.ArchivedDate).ToList();
        }

        public async Task<bool> ArchiveBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.IsArchived = true;
            booking.ArchivedDate = DateTime.Now;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> RestoreBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.IsArchived = false;
            booking.ArchivedDate = null;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> DeletePermanentlyAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            // Free up the time slot
            var timeSlot = await _timeSlotRepository.GetByIdAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = true;
                await _timeSlotRepository.UpdateAsync(timeSlot);
            }

            await _bookingRepository.DeleteAsync(booking);
            return true;
        }

        public async Task<bool> EmptyRecycleBinAsync(string ownerId)
        {
            var archivedBookings = await GetArchivedBookingsByOwnerIdAsync(ownerId);

            foreach (var booking in archivedBookings)
            {
                await DeletePermanentlyAsync(booking.Id);
            }

            return true;
        }
    }
}