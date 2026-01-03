using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Booking;
using EhjozProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Stadium)
                .Include(b => b.TimeSlot)
                .Include(b => b.Payment)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Stadium)
                .Include(b => b.TimeSlot)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Stadium)
                .Include(b => b.TimeSlot)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByStadiumIdAsync(int stadiumId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.TimeSlot)
                .Include(b => b.Payment)
                .Where(b => b.StadiumId == stadiumId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Stadium)
                .Include(b => b.TimeSlot)
                .Include(b => b.Payment)
                .Where(b => b.Stadium.OwnerId == ownerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task DeleteAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}