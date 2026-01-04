namespace EhjozProject.Web.ViewModels.Admin
{
    public class BookingManagementViewModel
    {
        public List<BookingViewModel> Bookings { get; set; } = new();
    }

    public class BookingViewModel
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string StadiumName { get; set; } = null!;
        public string? Notes { get; set; }
        public bool IsArchived { get; set; }
    }
}

