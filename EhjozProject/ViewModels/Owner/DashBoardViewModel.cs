namespace EhjozProject.Web.ViewModels.Owner
{
    public class DashboardViewModel
    {
        public string OwnerName { get; set; } = null!;
        public string? OwnerEmail { get; set; }

        // Statistics
        public int TotalStadiums { get; set; }
        public int ActiveStadiums { get; set; }
        public int InactiveStadiums { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal MonthlyEarnings { get; set; }

        // Subscription Info
        public bool HasActiveSubscription { get; set; }
        public string? SubscriptionPlan { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int DaysRemaining { get; set; }

        // Recent Data
        public List<RecentBookingViewModel> RecentBookings { get; set; } = new();
        public List<StadiumSummaryViewModel> Stadiums { get; set; } = new();
    }

    public class RecentBookingViewModel
    {
        public int Id { get; set; }
        public string StadiumName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class StadiumSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public bool IsActive { get; set; }
        public int TotalBookings { get; set; }
        public string? ImageUrl { get; set; }
    }
}