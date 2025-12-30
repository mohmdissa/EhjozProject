using Microsoft.AspNetCore.Identity;

namespace EhjozProject.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string ?FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ProfileImage { get; set; }

        // Role as string: "Customer", "Owner", "Admin"
        public string Role { get; set; } = "Customer";

        // Owner-specific
        public bool? IsApproved { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        // Navigation - PLURAL names for collections
        public virtual ICollection<Stadium.Stadium>? Stadiums { get; set; }
        public virtual ICollection<Booking.Booking>? Bookings { get; set; }
    }
}