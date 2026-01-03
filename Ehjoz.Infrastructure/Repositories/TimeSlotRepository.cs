using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Stadium;
using EhjozProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Infrastructure.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimeSlot>> GetAllAsync()
        {
            return await _context.TimeSlots
                .Include(t => t.Stadium)
                .Include(t => t.Booking)
                .ToListAsync();
        }

        public async Task<TimeSlot?> GetByIdAsync(int id)
        {
            return await _context.TimeSlots
                .Include(t => t.Stadium)
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TimeSlot>> GetByStadiumIdAsync(int stadiumId)
        {
            return await _context.TimeSlots
                .Include(t => t.Booking)
                .Where(t => t.StadiumId == stadiumId)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetByStadiumIdAndDateAsync(int stadiumId, DateOnly date)
        {
            return await _context.TimeSlots
                .Include(t => t.Booking)
                .Where(t => t.StadiumId == stadiumId && t.Date == date)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableByStadiumIdAsync(int stadiumId)
        {
            return await _context.TimeSlots
                .Where(t => t.StadiumId == stadiumId && t.IsAvailable && t.Date >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(t => t.Date)
                .ThenBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot> AddAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            return timeSlot;
        }

        public async Task<TimeSlot> UpdateAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
            return timeSlot;
        }

        public async Task DeleteAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
        }
    }
}