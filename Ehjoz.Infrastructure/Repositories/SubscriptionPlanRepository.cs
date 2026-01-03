using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Subscription;
using EhjozProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Infrastructure.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans
                .ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan subscriptionPlan)
        {
            _context.SubscriptionPlans.Add(subscriptionPlan);
            await _context.SaveChangesAsync();
            return subscriptionPlan;
        }

        public async Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan)
        {
            _context.SubscriptionPlans.Update(subscriptionPlan);
            await _context.SaveChangesAsync();
            return subscriptionPlan;
        }

        public async Task DeleteAsync(SubscriptionPlan subscriptionPlan)
        {
            _context.SubscriptionPlans.Remove(subscriptionPlan);
            await _context.SaveChangesAsync();
        }
    }
}

