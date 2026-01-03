namespace EhjozProject.Web.ViewModels.Admin
{
    public class OwnerManagementViewModel
    {
        public List<OwnerViewModel> Owners { get; set; } = new();
    }

    public class OwnerViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public bool IsApproved { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}

