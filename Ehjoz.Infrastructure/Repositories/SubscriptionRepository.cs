using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Subscription;
using EhjozProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Owner)
                .Include(s => s.Plan)
                .ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            return await _context.Subscriptions
                .Include(s => s.Owner)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Subscriptions
                .Include(s => s.Plan)
                .Where(s => s.OwnerId == ownerId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task DeleteAsync(Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }
    }
}

