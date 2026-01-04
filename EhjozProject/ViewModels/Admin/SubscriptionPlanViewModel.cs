namespace EhjozProject.Web.ViewModels.Admin
{
    public class SubscriptionPlanManagementViewModel
    {
        public List<SubscriptionPlanViewModel> Plans { get; set; } = new();
        public CreateSubscriptionPlanViewModel CreatePlan { get; set; } = new();
    }

    public class SubscriptionPlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int MaxStadiums { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubscriptionPlanViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int MaxStadiums { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SubscriptionManagementViewModel
    {
        public List<SubscriptionViewModel> Subscriptions { get; set; } = new();
    }

    public class SubscriptionViewModel
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = null!;
        public string PlanName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

