using EhjozProject.Domain.Models.Identity;

namespace EhjozProject.Domain.Models.Stadium
{
    public class Stadium
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Foreign Key
        public string OwnerId { get; set; } = null!;

        // Navigation - PLURAL names for collections
        public virtual ApplicationUser Owner { get; set; } = null!;
        public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public virtual ICollection<Booking.Booking> Bookings { get; set; } = new List<Booking.Booking>();
    }
}