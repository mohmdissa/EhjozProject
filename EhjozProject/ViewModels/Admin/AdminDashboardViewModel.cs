namespace EhjozProject.Web.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOwners { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingOwnerApprovals { get; set; }
        public int TotalStadiums { get; set; }
        public int ActiveStadiums { get; set; }
        public int TotalBookings { get; set; }
        public int TotalSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int TotalPlans { get; set; }
    }
}

