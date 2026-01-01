using EhjozProject.Domain.Models.Stadium;

namespace EhjozProject.Domain.Interfaces.Repositories
{
    public interface IStadiumRepository
    {
        Task<IEnumerable<Stadium>> GetAllAsync();
        Task<Stadium?> GetByIdAsync(int id);
        Task<IEnumerable<Stadium>> GetByOwnerIdAsync(string ownerId);
        Task<Stadium> AddAsync(Stadium stadium);
        Task<Stadium> UpdateAsync(Stadium stadium);
        Task DeleteAsync(Stadium stadium);
    }
}