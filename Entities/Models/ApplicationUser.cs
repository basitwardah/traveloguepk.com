using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Subscription fields
        public bool IsSubscribed { get; set; } = false;
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string? SubscriptionPlan { get; set; } // Monthly, Yearly, Lifetime

        // Computed property
        public bool HasActiveSubscription => IsSubscribed && 
                                            SubscriptionEndDate.HasValue && 
                                            SubscriptionEndDate.Value > DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Guide> CreatedGuides { get; set; } = new List<Guide>();
        public virtual ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();
    }
}

