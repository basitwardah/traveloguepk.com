using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Repository.Data;
using magazine_app.ViewModels;
using magazine_app.Services.Interfaces;

namespace magazine_app.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuideService _guideService;

        public UserDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IGuideService guideService)
        {
            _context = context;
            _userManager = userManager;
            _guideService = guideService;
        }

        // GET: UserDashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get favorites
            var favoriteGuideIds = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Select(f => f.GuideId)
                .ToListAsync();

            var allGuides = await _guideService.GetPublishedGuidesAsync();
            var favoriteMagazines = allGuides.Where(g => favoriteGuideIds.Contains(g.Id)).ToList();
            
            // Get recommended (non-favorite magazines)
            var recommendedMagazines = allGuides
                .Where(g => !favoriteGuideIds.Contains(g.Id))
                .Take(6)
                .ToList();
            
            // Get recently added
            var recentlyAdded = allGuides
                .OrderByDescending(g => g.CreatedAt)
                .Take(6)
                .ToList();
            
            // Calculate days until expiry
            var daysUntilExpiry = 0;
            if (user.HasActiveSubscription && user.SubscriptionEndDate.HasValue)
            {
                daysUntilExpiry = (int)(user.SubscriptionEndDate.Value - DateTime.UtcNow).TotalDays;
            }

            // Get total read count
            var totalRead = await _context.UserActivities
                .Where(a => a.UserId == user.Id && a.Action == "Read Magazine")
                .Select(a => a.GuideId)
                .Distinct()
                .CountAsync();

            var viewModel = new UserDashboardViewModel
            {
                UserName = user.FullName ?? user.UserName ?? "User",
                Email = user.Email ?? "",
                IsSubscribed = user.IsSubscribed,
                SubscriptionEndDate = user.SubscriptionEndDate,
                SubscriptionPlan = user.SubscriptionPlan,
                HasActiveSubscription = user.HasActiveSubscription,
                DaysUntilExpiry = daysUntilExpiry,
                FavoriteMagazines = favoriteMagazines,
                RecommendedMagazines = recommendedMagazines,
                RecentlyAdded = recentlyAdded,
                TotalFavorites = favoriteMagazines.Count,
                TotalRead = totalRead
            };

            return View(viewModel);
        }

        // GET: UserDashboard/Favorites
        public async Task<IActionResult> Favorites()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favoriteGuideIds = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Select(f => f.GuideId)
                .ToListAsync();

            var allGuides = await _guideService.GetPublishedGuidesAsync();
            var favoriteMagazines = allGuides.Where(g => favoriteGuideIds.Contains(g.Id)).ToList();

            return View(favoriteMagazines);
        }

        // POST: UserDashboardController/AddToFavorites/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Check if already favorited
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == user.Id && f.GuideId == id);

            if (exists)
            {
                return Json(new { success = false, message = "Already in favorites" });
            }

            var favorite = new Favorite
            {
                UserId = user.Id,
                GuideId = id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Added to favorites!" });
        }

        // POST: UserDashboard/RemoveFromFavorites/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.GuideId == id);

            if (favorite == null)
            {
                return Json(new { success = false, message = "Not in favorites" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Removed from favorites!" });
        }
    }
}

