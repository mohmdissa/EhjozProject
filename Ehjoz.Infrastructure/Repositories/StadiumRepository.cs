using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Stadium;
using EhjozProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Infrastructure.Repositories
{
    public class StadiumRepository : IStadiumRepository
    {
        private readonly ApplicationDbContext _context;

        public StadiumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stadium>> GetAllAsync()
        {
            return await _context.Stadiums
                .Include(s => s.Owner)
                .Include(s => s.TimeSlots)
                .ToListAsync();
        }

        public async Task<Stadium?> GetByIdAsync(int id)
        {
            return await _context.Stadiums
                .Include(s => s.Owner)
                .Include(s => s.TimeSlots)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Stadium>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Stadiums
                .Include(s => s.TimeSlots)
                .Include(s => s.Bookings)
                .Where(s => s.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Stadium> AddAsync(Stadium stadium)
        {
            await _context.Stadiums.AddAsync(stadium);
            await _context.SaveChangesAsync();
            return stadium;
        }

        public async Task<Stadium> UpdateAsync(Stadium stadium)
        {
            _context.Stadiums.Update(stadium);
            await _context.SaveChangesAsync();
            return stadium;
        }

        public async Task DeleteAsync(Stadium stadium)
        {
            _context.Stadiums.Remove(stadium);
            await _context.SaveChangesAsync();
        }
    }
}
