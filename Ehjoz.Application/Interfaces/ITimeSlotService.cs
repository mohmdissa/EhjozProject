using EhjozProject.Domain.Models.Stadium;

namespace EhjozProject.Application.Interfaces
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync();
        Task<TimeSlot?> GetTimeSlotByIdAsync(int id);
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByStadiumIdAsync(int stadiumId);
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByStadiumIdAndDateAsync(int stadiumId, DateOnly date);
        Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsByStadiumIdAsync(int stadiumId);
        Task<TimeSlot> CreateTimeSlotAsync(TimeSlot timeSlot);
        Task<TimeSlot> UpdateTimeSlotAsync(TimeSlot timeSlot);
        Task<bool> DeleteTimeSlotAsync(int id);
        Task<bool> MarkAsBookedAsync(int id);
        Task<bool> MarkAsAvailableAsync(int id);
    }
}