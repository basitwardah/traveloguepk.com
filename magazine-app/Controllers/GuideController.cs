using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using magazine_app.Models;
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
            var guides = await _guideService.GetPublishedGuidesAsync();
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

            // Log activity
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _activityService.LogActivityAsync(
                    user.Id,
                    "Read Guide",
                    guide.Id,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );
            }

            return View(guide);
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

            var filePath = _fileService.GetFullPath(guide.PdfPath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Log activity
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _activityService.LogActivityAsync(
                    user.Id,
                    "Download PDF",
                    guide.Id,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );
            }

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

