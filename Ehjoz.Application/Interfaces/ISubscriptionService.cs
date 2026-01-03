using EhjozProject.Domain.Models.Subscription;

namespace EhjozProject.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription?> GetSubscriptionByIdAsync(int id);
        Task<IEnumerable<Subscription>> GetSubscriptionsByOwnerIdAsync(string ownerId);
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription> UpdateSubscriptionAsync(Subscription subscription);
        Task<bool> DeleteSubscriptionAsync(int id);
        
        Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync();
        Task<SubscriptionPlan?> GetSubscriptionPlanByIdAsync(int id);
        Task<SubscriptionPlan> CreateSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan);
        Task<SubscriptionPlan> UpdateSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan);
        Task<bool> DeleteSubscriptionPlanAsync(int id);
    }
}

