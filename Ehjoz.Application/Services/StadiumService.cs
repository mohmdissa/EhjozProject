using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Stadium;

namespace EhjozProject.Application.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly IStadiumRepository _stadiumRepository;

        public StadiumService(IStadiumRepository stadiumRepository)
        {
            _stadiumRepository = stadiumRepository;
        }

        public async Task<IEnumerable<Stadium>> GetAllStadiumsAsync()
        {
            var stadiums = await _stadiumRepository.GetAllAsync();
            return stadiums ?? new List<Stadium>();
        }

        public async Task<IEnumerable<Stadium>> GetFeaturedStadiumsAsync(int count = 6)
        {
            var stadiums = await _stadiumRepository.GetAllAsync();

            if (stadiums == null)
                return new List<Stadium>();

            return stadiums.Where(s => s.IsActive).Take(count).ToList();
        }

        public async Task<IEnumerable<Stadium>> GetStadiumsByOwnerIdAsync(string ownerId)
        {
            var stadiums = await _stadiumRepository.GetByOwnerIdAsync(ownerId);
            return stadiums ?? new List<Stadium>();
        }

        public async Task<IEnumerable<Stadium>> GetStadiumsByCityAsync(string city)
        {
            var stadiums = await _stadiumRepository.GetAllAsync();

            if (stadiums == null)
                return new List<Stadium>();

            return stadiums.Where(s => s.City.ToLower() == city.ToLower()).ToList();
        }

        public async Task<Stadium?> GetStadiumByIdAsync(int id)
        {
            return await _stadiumRepository.GetByIdAsync(id);
        }

        public async Task<Stadium> CreateStadiumAsync(Stadium stadium)
        {
            return await _stadiumRepository.AddAsync(stadium);
        }

        public async Task<Stadium> UpdateStadiumAsync(Stadium stadium)
        {
            return await _stadiumRepository.UpdateAsync(stadium);
        }

        public async Task<bool> DeleteStadiumAsync(int id)
        {
            var stadium = await _stadiumRepository.GetByIdAsync(id);
            if (stadium == null) return false;

            await _stadiumRepository.DeleteAsync(stadium);
            return true;
        }

        public async Task<bool> ToggleStadiumStatusAsync(int id)
        {
            var stadium = await _stadiumRepository.GetByIdAsync(id);
            if (stadium == null) return false;

            stadium.IsActive = !stadium.IsActive;
            await _stadiumRepository.UpdateAsync(stadium);
            return true;
        }
    }
}