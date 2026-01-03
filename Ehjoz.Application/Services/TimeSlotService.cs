using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Domain.Models.Stadium;

namespace EhjozProject.Application.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _timeSlotRepository;

        public TimeSlotService(ITimeSlotRepository timeSlotRepository)
        {
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync()
        {
            return await _timeSlotRepository.GetAllAsync() ?? new List<TimeSlot>();
        }

        public async Task<TimeSlot?> GetTimeSlotByIdAsync(int id)
        {
            return await _timeSlotRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByStadiumIdAsync(int stadiumId)
        {
            return await _timeSlotRepository.GetByStadiumIdAsync(stadiumId) ?? new List<TimeSlot>();
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByStadiumIdAndDateAsync(int stadiumId, DateOnly date)
        {
            return await _timeSlotRepository.GetByStadiumIdAndDateAsync(stadiumId, date) ?? new List<TimeSlot>();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsByStadiumIdAsync(int stadiumId)
        {
            return await _timeSlotRepository.GetAvailableByStadiumIdAsync(stadiumId) ?? new List<TimeSlot>();
        }

        public async Task<TimeSlot> CreateTimeSlotAsync(TimeSlot timeSlot)
        {
            timeSlot.IsAvailable = true;
            return await _timeSlotRepository.AddAsync(timeSlot);
        }

        public async Task<TimeSlot> UpdateTimeSlotAsync(TimeSlot timeSlot)
        {
            return await _timeSlotRepository.UpdateAsync(timeSlot);
        }

        public async Task<bool> DeleteTimeSlotAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(id);
            if (timeSlot == null) return false;

            await _timeSlotRepository.DeleteAsync(timeSlot);
            return true;
        }

        public async Task<bool> MarkAsBookedAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(id);
            if (timeSlot == null) return false;

            timeSlot.IsAvailable = false;
            await _timeSlotRepository.UpdateAsync(timeSlot);
            return true;
        }

        public async Task<bool> MarkAsAvailableAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(id);
            if (timeSlot == null) return false;

            timeSlot.IsAvailable = true;
            await _timeSlotRepository.UpdateAsync(timeSlot);
            return true;
        }
    }
}