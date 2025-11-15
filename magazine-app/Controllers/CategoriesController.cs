using Entities.Models;
using magazine_app.Services.Interfaces;
using magazine_app.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace magazine_app.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IGuideService _guideService;
        private readonly IFileService _fileService;
        private readonly IActivityService _activityService;
        private readonly ILogService _logService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CategoriesController(
            IGuideService guideService,
            IFileService fileService,
            IActivityService activityService,
            ILogService logService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _guideService = guideService;
            _fileService = fileService;
            _activityService = activityService;
            _logService = logService;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(string category = "all")
        {
            // Normalize category parameter
            if (string.IsNullOrWhiteSpace(category))
            {
                category = "all";
            }
            category = category.ToLower().Trim();
            
            // Get current user ID if logged in
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            
            // Get guides filtered by category with favorite status
            var guides = await _guideService.GetPublishedGuidesByCategoryAsync(category, userId);
            var guidesList = guides.ToList(); // Convert to list to ensure it's materialized
            
            // Get ALL categories from database (not just ones with magazines)
            var allCategories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
            
            // Get all published guides for counting
            var allGuides = await _guideService.GetPublishedGuidesAsync();
            var allGuidesList = allGuides.ToList();
            
            // Build category list with counts - use a proper class instead of anonymous type
            var categoriesWithCounts = allCategories.Select(cat => new CategoryCountViewModel
            {
                Name = cat.Name,
                Slug = cat.Slug,
                Count = allGuidesList.Count(g => g.CategorySlug != null && g.CategorySlug.ToLower() == cat.Slug.ToLower())
            }).ToList();
            
            // Pass data to view
            ViewBag.CurrentCategory = category;
            ViewBag.IsUserLoggedIn = User.Identity?.IsAuthenticated ?? false;
            ViewBag.CategoriesWithCounts = categoriesWithCounts;
            ViewBag.TotalCount = allGuidesList.Count;
            
            // Log for debugging
            await _logService.LogInfoAsync(
                $"Categories page - Category: '{category}', Total Published: {allGuidesList.Count}, Filtered: {guidesList.Count}, Categories: {allCategories.Count}", 
                "CategoriesController");
            
            return View(guidesList);
        }
    }
}

