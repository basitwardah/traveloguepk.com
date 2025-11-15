namespace magazine_app.ViewModels
{
    public class UserDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSubscribed { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string? SubscriptionPlan { get; set; }
        public bool HasActiveSubscription { get; set; }
        public int DaysUntilExpiry { get; set; }
        
        public List<GuideListViewModel> FavoriteMagazines { get; set; } = new();
        public List<GuideListViewModel> RecommendedMagazines { get; set; } = new();
        public List<GuideListViewModel> RecentlyAdded { get; set; } = new();
        
        public int TotalFavorites { get; set; }
        public int TotalRead { get; set; }
    }
}

