using Entities.Models;
using magazine_app.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace magazine_app.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuideService _guideService;
        private readonly ILogService _logService;

        public FavoriteController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IGuideService guideService,
            ILogService logService)
        {
            _context = context;
            _userManager = userManager;
            _guideService = guideService;
            _logService = logService;
        }

        // GET: /Favorite/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user's favorited guides
            var favoritedGuides = await _context.Favorites
                .Include(f => f.Guide)
                    .ThenInclude(g => g.Category)
                .Include(f => f.Guide)
                    .ThenInclude(g => g.CreatedBy)
                .Where(f => f.UserId == user.Id && f.Guide.IsPublished)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new magazine_app.ViewModels.GuideListViewModel
                {
                    Id = f.Guide.Id,
                    Slug = f.Guide.Slug,
                    Title = f.Guide.Title,
                    Summary = f.Guide.Summary,
                    CoverImagePath = f.Guide.CoverImagePath,
                    IsPublished = f.Guide.IsPublished,
                    CategoryId = f.Guide.CategoryId,
                    CategoryName = f.Guide.Category != null ? f.Guide.Category.Name : null,
                    CategorySlug = f.Guide.Category != null ? f.Guide.Category.Slug : null,
                    CurrentPrice = f.Guide.CurrentPrice,
                    OldPrice = f.Guide.OldPrice,
                    CreatedAt = f.Guide.CreatedAt,
                    CreatedByName = f.Guide.CreatedBy != null ? f.Guide.CreatedBy.FullName ?? f.Guide.CreatedBy.Email ?? "Unknown" : "Unknown",
                    IsFavorited = true
                })
                .ToListAsync();

            return View(favoritedGuides);
        }

        // POST: /Favorite/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int guideId, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Please login to add favorites" });
            }

            // Check if guide exists and is published
            var guide = await _context.Guides.FindAsync(guideId);
            if (guide == null || !guide.IsPublished)
            {
                return Json(new { success = false, message = "Magazine not found" });
            }

            // Check if already favorited
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.GuideId == guideId);

            if (existingFavorite != null)
            {
                return Json(new { success = false, message = "Already in favorites" });
            }

            // Add to favorites
            var favorite = new Favorite
            {
                UserId = user.Id,
                GuideId = guideId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            await _logService.LogInfoAsync($"User {user.Email} added guide {guide.Title} to favorites", "FavoriteController");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Json(new { success = true, message = "Added to favorites" });
        }

        // POST: /Favorite/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int guideId, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Please login" });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.GuideId == guideId);

            if (favorite == null)
            {
                return Json(new { success = false, message = "Not in favorites" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            var guide = await _context.Guides.FindAsync(guideId);
            await _logService.LogInfoAsync($"User {user.Email} removed guide {guide?.Title} from favorites", "FavoriteController");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Json(new { success = true, message = "Removed from favorites" });
        }

        // POST: /Favorite/Toggle (AJAX)
        [HttpPost]
        public async Task<IActionResult> Toggle(int guideId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Please login to add favorites", isAuthenticated = false });
            }

            // Check if guide exists
            var guide = await _context.Guides.FindAsync(guideId);
            if (guide == null)
            {
                return Json(new { success = false, message = "Magazine not found" });
            }

            // Check if already favorited
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.GuideId == guideId);

            if (favorite != null)
            {
                // Remove from favorites
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                await _logService.LogInfoAsync($"User {user.Email} removed guide {guide.Title} from favorites", "FavoriteController");
                return Json(new { success = true, isFavorited = false, message = "Removed from favorites" });
            }
            else
            {
                // Add to favorites
                var newFavorite = new Favorite
                {
                    UserId = user.Id,
                    GuideId = guideId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Favorites.Add(newFavorite);
                await _context.SaveChangesAsync();
                await _logService.LogInfoAsync($"User {user.Email} added guide {guide.Title} to favorites", "FavoriteController");
                return Json(new { success = true, isFavorited = true, message = "Added to favorites" });
            }
        }
    }
}

