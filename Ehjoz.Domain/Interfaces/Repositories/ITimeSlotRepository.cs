using EhjozProject.Domain.Models.Stadium;

namespace EhjozProject.Domain.Interfaces.Repositories
{
    public interface ITimeSlotRepository
    {
        Task<IEnumerable<TimeSlot>> GetAllAsync();
        Task<TimeSlot?> GetByIdAsync(int id);
        Task<IEnumerable<TimeSlot>> GetByStadiumIdAsync(int stadiumId);
        Task<IEnumerable<TimeSlot>> GetByStadiumIdAndDateAsync(int stadiumId, DateOnly date);
        Task<IEnumerable<TimeSlot>> GetAvailableByStadiumIdAsync(int stadiumId);
        Task<TimeSlot> AddAsync(TimeSlot timeSlot);
        Task<TimeSlot> UpdateAsync(TimeSlot timeSlot);
        Task DeleteAsync(TimeSlot timeSlot);
    }
}