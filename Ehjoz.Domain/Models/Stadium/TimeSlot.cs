namespace EhjozProject.Domain.Models.Stadium
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Foreign Key
        public int StadiumId { get; set; }

        // Navigation - SINGULAR for single reference
        public virtual Stadium Stadium { get; set; } = null!;
        public virtual Booking.Booking? Booking { get; set; }
    }
}