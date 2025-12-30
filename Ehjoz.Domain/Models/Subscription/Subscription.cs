using EhjozProject.Domain.Models.Identity;
using EhjozProject.Domain.Models.Subscription;

namespace EhjozProject.Domain.Models.Subscription
{
    public class Subscription
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // Foreign Keys
        public string OwnerId { get; set; } = null!;
        public int PlanId { get; set; }

        // Navigation - SINGULAR for single reference
        public virtual ApplicationUser Owner { get; set; } = null!;
        public virtual SubscriptionPlan Plan { get; set; } = null!;
    }
}