using System.Diagnostics;
using magazine_app.Models;
using magazine_app.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Entities.Models;

namespace magazine_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGuideService _guideService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IGuideService guideService, UserManager<ApplicationUser> userManager)
        {
            _guideService = guideService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user ID if logged in
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            
            // Get guides with favorite status
            var guides = userId != null 
                ? await _guideService.GetPublishedGuidesWithFavoritesAsync(userId)
                : await _guideService.GetPublishedGuidesAsync();
            
            // Get 4 featured magazines for home page
            var featuredGuides = guides.Take(4).ToList();
            
            // Pass authentication status to view
            ViewBag.IsUserLoggedIn = User.Identity?.IsAuthenticated ?? false;
            
            return View(featuredGuides);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
