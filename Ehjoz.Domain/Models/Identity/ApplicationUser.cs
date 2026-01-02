using Microsoft.AspNetCore.Identity;

namespace EhjozProject.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ProfileImage { get; set; }

        
        public string Role { get; set; } = "Customer";

        public bool? IsApproved { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        public ICollection<Stadium.Stadium> Stadiums { get; set; } = new List<Stadium.Stadium>();
        public ICollection<Booking.Booking> Bookings { get; set; } = new List<Booking.Booking>();
    }
}
