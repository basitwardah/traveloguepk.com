using Entities.Models;

namespace magazine_app.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int SubscribedUsers { get; set; }
        public int NonSubscribedUsers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalMagazines { get; set; }
        public int PublishedMagazines { get; set; }
        public int UnpublishedMagazines { get; set; }
        public int FreeMagazines { get; set; }
        public int PaidMagazines { get; set; }
        
        public List<UserListItemViewModel> RecentUsers { get; set; } = new();
        public List<GuideListViewModel> RecentMagazines { get; set; } = new();
        public List<ActivityViewModel> RecentActivities { get; set; } = new();
    }
    
    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSubscribed { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string? SubscriptionPlan { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool HasActiveSubscription { get; set; }
    }
    
    public class ActivityViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? GuideTitle { get; set; }
    }
}

