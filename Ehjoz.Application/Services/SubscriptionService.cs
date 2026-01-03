using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Subscription;

namespace EhjozProject.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetAllAsync();
            return subscriptions ?? new List<Subscription>();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int id)
        {
            return await _subscriptionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByOwnerIdAsync(string ownerId)
        {
            var subscriptions = await _subscriptionRepository.GetByOwnerIdAsync(ownerId);
            return subscriptions ?? new List<Subscription>();
        }

        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            return await _subscriptionRepository.AddAsync(subscription);
        }

        public async Task<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            return await _subscriptionRepository.UpdateAsync(subscription);
        }

        public async Task<bool> DeleteSubscriptionAsync(int id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            if (subscription == null) return false;

            await _subscriptionRepository.DeleteAsync(subscription);
            return true;
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync()
        {
            var plans = await _subscriptionPlanRepository.GetAllAsync();
            return plans ?? new List<SubscriptionPlan>();
        }

        public async Task<SubscriptionPlan?> GetSubscriptionPlanByIdAsync(int id)
        {
            return await _subscriptionPlanRepository.GetByIdAsync(id);
        }

        public async Task<SubscriptionPlan> CreateSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan)
        {
            return await _subscriptionPlanRepository.AddAsync(subscriptionPlan);
        }

        public async Task<SubscriptionPlan> UpdateSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan)
        {
            return await _subscriptionPlanRepository.UpdateAsync(subscriptionPlan);
        }

        public async Task<bool> DeleteSubscriptionPlanAsync(int id)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
            if (plan == null) return false;

            await _subscriptionPlanRepository.DeleteAsync(plan);
            return true;
        }
    }
}

