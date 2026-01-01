using EhjozProject.Domain.Models.Stadium;

namespace EhjozProject.Application.Interfaces
{
    public interface IStadiumService
    {
        Task<IEnumerable<Stadium>> GetAllStadiumsAsync();
        Task<IEnumerable<Stadium>> GetFeaturedStadiumsAsync(int count = 6);
        Task<IEnumerable<Stadium>> GetStadiumsByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Stadium>> GetStadiumsByCityAsync(string city);
        Task<Stadium?> GetStadiumByIdAsync(int id);
        Task<Stadium> CreateStadiumAsync(Stadium stadium);
        Task<Stadium> UpdateStadiumAsync(Stadium stadium);
        Task<bool> DeleteStadiumAsync(int id);
        Task<bool> ToggleStadiumStatusAsync(int id);
    }
}