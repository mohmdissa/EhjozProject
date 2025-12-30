namespace EhjozProject.Domain.Models.Payment
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public bool IsPaid { get; set; } = false;

        // Foreign Key
        public int BookingId { get; set; }

        // Navigation - SINGULAR for single reference
        public virtual Booking.Booking Booking { get; set; } = null!;
    }
}