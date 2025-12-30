using EhjozProject.Domain.Models.Identity;

namespace EhjozProject.Domain.Models.Booking
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Status as string: "Pending", "Confirmed", "Cancelled", "Completed"
        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        // Foreign Keys
        public string UserId { get; set; } = null!;
        public int StadiumId { get; set; }
        public int TimeSlotId { get; set; }

        // Navigation - SINGULAR for single reference
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Stadium.Stadium Stadium { get; set; } = null!;
        public virtual Stadium.TimeSlot TimeSlot { get; set; } = null!;
        public virtual Payment.Payment? Payment { get; set; }
    }
}