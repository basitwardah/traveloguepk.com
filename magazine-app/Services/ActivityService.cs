using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Repository.Data;
using magazine_app.Services.Interfaces;

namespace magazine_app.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;

        public ActivityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string action, int? guideId = null, string? ipAddress = null, string? userAgent = null)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                Action = action,
                GuideId = guideId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.UserActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(string userId, int count = 50)
        {
            return await _context.UserActivities
                .Include(a => a.Guide)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetGuideActivitiesAsync(int guideId, int count = 50)
        {
            return await _context.UserActivities
                .Include(a => a.User)
                .Include(a => a.Guide)
                .Where(a => a.GuideId == guideId)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(int count = 100)
        {
            return await _context.UserActivities
                .Include(a => a.User)
                .Include(a => a.Guide)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetActivityCountByActionAsync(string action, DateTime? fromDate = null)
        {
            var query = _context.UserActivities.Where(a => a.Action == action);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            }

            return await query.CountAsync();
        }
    }
}

