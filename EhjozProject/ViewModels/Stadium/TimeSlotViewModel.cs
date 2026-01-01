using System.ComponentModel.DataAnnotations;

namespace EhjozProject.Web.ViewModels.Stadium
{
    public class TimeSlotViewModel
    {
        public int Id { get; set; }

        [Required]
        public int StadiumId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeOnly EndTime { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;
    }
}