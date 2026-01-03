using EhjozProject.Domain.Models.Identity;
using EhjozProject.Domain.Models.Stadium;
using EhjozProject.Domain.Models.Payment;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EhjozProject.Domain.Models.Booking
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [StringLength(500)]
        public string? Notes { get; set; }

        // Archive/Recycle Bin Properties
        public bool ?IsArchived { get; set; } = false;
        public DateTime? ArchivedDate { get; set; }

        // Foreign Keys
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int StadiumId { get; set; }

        [Required]
        public int TimeSlotId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("StadiumId")]
        public virtual Stadium.Stadium? Stadium { get; set; }

        [ForeignKey("TimeSlotId")]
        public virtual TimeSlot? TimeSlot { get; set; }

        public virtual Payment.Payment? Payment { get; set; }
    }
}