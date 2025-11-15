using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using magazine_app.Services.Interfaces;

namespace magazine_app.Controllers
{
    public class GuideController : Controller
    {
        private readonly IGuideService _guideService;
        private readonly IActivityService _activityService;
        private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public GuideController(
            IGuideService guideService,
            IActivityService activityService,
            IFileService fileService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _guideService = guideService;
            _activityService = activityService;
            _fileService = fileService;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Guide/Index - List all published guides
        public async Task<IActionResult> Index()
        {
            // Get current user ID if logged in
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            
            // Get guides with favorite status
            var guides = userId != null 
                ? await _guideService.GetPublishedGuidesWithFavoritesAsync(userId)
                : await _guideService.GetPublishedGuidesAsync();
            
            // Pass authentication status to view
            ViewBag.IsUserLoggedIn = User.Identity?.IsAuthenticated ?? false;
            
            return View(guides);
        }

        // GET: Guide/Detail/slug - Show cover and summary
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var guide = await _guideService.GetGuideBySlugAsync(slug);
            if (guide == null || !guide.IsPublished)
            {
                return NotFound();
            }

            return View(guide);
        }

        // GET: Guide/Read/slug - Show PDF viewer (Requires authentication)
        [Authorize]
        public async Task<IActionResult> Read(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var guide = await _guideService.GetGuideBySlugAsync(slug);
            if (guide == null || !guide.IsPublished)
            {
                return NotFound();
            }

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Read", "Guide", new { slug }) });
            }

            // Check access permission
            var hasAccess = await CheckMagazineAccess(user, guide);
            
            if (!hasAccess)
            {
                // Determine why access was denied
                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Any(r => r == "Admin" || r == "SuperAdmin" || r == "Uploader");
                
                if (!isAdmin)
                {
                    if (user.HasActiveSubscription)
                    {
                        // Has subscription but still blocked - shouldn't happen
                        TempData["Error"] = "Access error. Please contact support.";
                    }
                    else if (guide.CurrentPrice > 0)
                    {
                        // Paid magazine, no subscription
                        TempData["Error"] = $"This magazine requires a subscription or one-time purchase of ${guide.CurrentPrice}. Subscribe now for unlimited access!";
                        TempData["NeedSubscription"] = true;
                        TempData["MagazinePrice"] = guide.CurrentPrice;
                    }
                }
                
                return RedirectToAction("Detail", new { slug });
            }

            // Log activity
            await _activityService.LogActivityAsync(
                user.Id,
                "Read Magazine",
                guide.Id,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers["User-Agent"].ToString()
            );

            return View(guide);
        }

        // Helper method to check magazine access
        private async Task<bool> CheckMagazineAccess(ApplicationUser user, magazine_app.ViewModels.GuideDetailViewModel guide)
        {
            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            
            // Admin, SuperAdmin, and Uploader have full access
            if (userRoles.Any(r => r == "Admin" || r == "SuperAdmin" || r == "Uploader"))
            {
                return true;
            }

            // Check if user has active subscription
            if (user.HasActiveSubscription)
            {
                return true;
            }

            // Check if magazine is free
            if (guide.CurrentPrice == 0)
            {
                return true;
            }

            // No access - paid magazine and no subscription
            return false;
        }

        // GET: Guide/DownloadPdf/slug - Download PDF (Requires authentication)
        [Authorize]
        public async Task<IActionResult> DownloadPdf(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var guide = await _guideService.GetGuideBySlugAsync(slug);
            if (guide == null || !guide.IsPublished)
            {
                return NotFound();
            }

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check access permission
            var hasAccess = await CheckMagazineAccess(user, guide);
            
            if (!hasAccess)
            {
                TempData["Error"] = "You don't have access to download this magazine. Please subscribe or purchase it.";
                return RedirectToAction("Detail", new { slug });
            }

            var filePath = _fileService.GetFullPath(guide.PdfPath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Log activity
            await _activityService.LogActivityAsync(
                user.Id,
                "Download PDF",
                guide.Id,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers["User-Agent"].ToString()
            );

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var fileName = $"{guide.Slug}.pdf";
            return File(memory, "application/pdf", fileName);
        }
    }
}

