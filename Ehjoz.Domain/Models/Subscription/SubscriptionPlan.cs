namespace EhjozProject.Domain.Models.Subscription
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int MaxStadiums { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation - PLURAL for collection
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}