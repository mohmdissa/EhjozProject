using System.ComponentModel.DataAnnotations;

namespace EhjozProject.Web.ViewModels.Booking
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        [Required]
        public int StadiumId { get; set; }

        [Required]
        public int TimeSlotId { get; set; }

        [Display(Name = "Stadium Name")]
        public string? StadiumName { get; set; }

        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Start Time")]
        public string? StartTime { get; set; }

        [Display(Name = "End Time")]
        public string? EndTime { get; set; }

        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}