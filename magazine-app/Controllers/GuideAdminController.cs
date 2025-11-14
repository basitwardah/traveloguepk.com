using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using magazine_app.Models;
using magazine_app.ViewModels;
using magazine_app.Services.Interfaces;

namespace magazine_app.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Uploader")]
    public class GuideAdminController : Controller
    {
        private readonly IGuideService _guideService;
        private readonly IFileService _fileService;
        private readonly IActivityService _activityService;
        private readonly ILogService _logService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GuideAdminController(
            IGuideService guideService,
            IFileService fileService,
            IActivityService activityService,
            ILogService logService,
            UserManager<ApplicationUser> userManager)
        {
            _guideService = guideService;
            _fileService = fileService;
            _activityService = activityService;
            _logService = logService;
            _userManager = userManager;
        }

        // GET: GuideAdmin/Index
        public async Task<IActionResult> Index()
        {
            var guides = await _guideService.GetAllGuidesAsync();
            return View(guides);
        }

        // GET: GuideAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var guide = await _guideService.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return NotFound();
            }

            return View(guide);
        }

        // GET: GuideAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GuideAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GuideCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate files
            if (!_fileService.ValidateCoverImage(model.CoverImage))
            {
                ModelState.AddModelError("CoverImage", $"Invalid cover image. Allowed formats: jpg, png, webp. Max size: {_fileService.MaxCoverSizeMB}MB");
                return View(model);
            }

            if (!_fileService.ValidatePdf(model.PdfFile))
            {
                ModelState.AddModelError("PdfFile", $"Invalid PDF file. Max size: {_fileService.MaxPdfSizeMB}MB");
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var guide = await _guideService.CreateGuideAsync(model, user.Id);
                
                if (guide == null)
                {
                    ModelState.AddModelError("", "Failed to create magazine. Please try again.");
                    return View(model);
                }

                // Log activity
                await _activityService.LogActivityAsync(
                    user.Id,
                    "Create Guide",
                    guide.Id,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Success"] = $"Magazine '{guide.Title}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error creating guide: {model.Title}", ex.Message, "GuideAdminController");
                ModelState.AddModelError("", "An error occurred while creating the magazine. Please try again.");
                return View(model);
            }
        }

        // GET: GuideAdmin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var guide = await _guideService.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return NotFound();
            }

            var model = new GuideEditViewModel
            {
                Id = guide.Id,
                Title = guide.Title,
                Summary = guide.Summary,
                IsPublished = guide.IsPublished,
                ExistingCoverImagePath = guide.CoverImagePath,
                ExistingPdfPath = guide.PdfPath,
                Slug = guide.Slug
            };

            return View(model);
        }

        // POST: GuideAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GuideEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate files if provided
            if (model.CoverImage != null && !_fileService.ValidateCoverImage(model.CoverImage))
            {
                ModelState.AddModelError("CoverImage", $"Invalid cover image. Allowed formats: jpg, png, webp. Max size: {_fileService.MaxCoverSizeMB}MB");
                return View(model);
            }

            if (model.PdfFile != null && !_fileService.ValidatePdf(model.PdfFile))
            {
                ModelState.AddModelError("PdfFile", $"Invalid PDF file. Max size: {_fileService.MaxPdfSizeMB}MB");
                return View(model);
            }

            try
            {
                var success = await _guideService.UpdateGuideAsync(id, model);
                if (!success)
                {
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    await _activityService.LogActivityAsync(
                        user.Id,
                        "Update Guide",
                        id,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                TempData["Success"] = "Magazine updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error updating guide ID: {id}", ex.Message, "GuideAdminController");
                ModelState.AddModelError("", "An error occurred while updating the magazine. Please try again.");
                return View(model);
            }
        }

        // POST: GuideAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _guideService.DeleteGuideAsync(id);
                if (!success)
                {
                    TempData["Error"] = "Magazine not found.";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    await _activityService.LogActivityAsync(
                        user.Id,
                        "Delete Guide",
                        id,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                TempData["Success"] = "Magazine deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error deleting guide ID: {id}", ex.Message, "GuideAdminController");
                TempData["Error"] = "An error occurred while deleting the magazine.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: GuideAdmin/TogglePublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            try
            {
                var success = await _guideService.TogglePublishStatusAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    await _activityService.LogActivityAsync(
                        user.Id,
                        "Toggle Publish Status",
                        id,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                TempData["Success"] = "Publish status updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error toggling publish status for guide ID: {id}", ex.Message, "GuideAdminController");
                TempData["Error"] = "An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

