using EhjozProject.Domain.Models.Subscription;

namespace EhjozProject.Domain.Interfaces.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task<IEnumerable<SubscriptionPlan>> GetAllAsync();
        Task<SubscriptionPlan?> GetByIdAsync(int id);
        Task<SubscriptionPlan> AddAsync(SubscriptionPlan subscriptionPlan);
        Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan);
        Task DeleteAsync(SubscriptionPlan subscriptionPlan);
    }
}

