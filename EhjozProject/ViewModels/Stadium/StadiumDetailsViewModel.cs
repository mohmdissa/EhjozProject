using System.ComponentModel.DataAnnotations;

namespace EhjozProject.Web.ViewModels.Stadium
{
    public class StadiumDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Stadium Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Display(Name = "City")]
        public string City { get; set; } = null!;

        [Display(Name = "Price Per Hour")]
        [DataType(DataType.Currency)]
        public decimal PricePerHour { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Owner Name")]
        public string? OwnerName { get; set; }

        [Display(Name = "Owner Phone")]
        public string? OwnerPhone { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        // Available time slots for booking
        public List<TimeSlotViewModel> AvailableTimeSlots { get; set; } = new();
    }
}