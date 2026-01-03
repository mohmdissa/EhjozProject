using EhjozProject.Domain.Models.Subscription;

namespace EhjozProject.Domain.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(int id);
        Task<IEnumerable<Subscription>> GetByOwnerIdAsync(string ownerId);
        Task<Subscription> AddAsync(Subscription subscription);
        Task<Subscription> UpdateAsync(Subscription subscription);
        Task DeleteAsync(Subscription subscription);
    }
}

