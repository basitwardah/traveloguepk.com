using Entities.Models;

namespace magazine_app.Services.Interfaces
{
    public interface IActivityService
    {
        Task LogActivityAsync(string userId, string action, int? guideId = null, string? ipAddress = null, string? userAgent = null);
        Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(string userId, int count = 50);
        Task<IEnumerable<UserActivity>> GetGuideActivitiesAsync(int guideId, int count = 50);
        Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(int count = 100);
        Task<int> GetActivityCountByActionAsync(string action, DateTime? fromDate = null);
    }
}

