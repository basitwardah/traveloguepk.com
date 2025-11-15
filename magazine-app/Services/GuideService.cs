using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Entities.Models;
using Repository.Data;
using magazine_app.ViewModels;
using magazine_app.Services.Interfaces;

namespace magazine_app.Services
{
    public class GuideService : IGuideService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogService _logService;

        public GuideService(ApplicationDbContext context, IFileService fileService, ILogService logService)
        {
            _context = context;
            _fileService = fileService;
            _logService = logService;
        }

        public async Task<IEnumerable<GuideListViewModel>> GetAllGuidesAsync()
        {
            return await _context.Guides
                .Include(g => g.CreatedBy)
                .Include(g => g.Category)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new GuideListViewModel
                {
                    Id = g.Id,
                    Slug = g.Slug,
                    Title = g.Title,
                    Summary = g.Summary,
                    CoverImagePath = g.CoverImagePath,
                    IsPublished = g.IsPublished,
                    CategoryId = g.CategoryId,
                    CategoryName = g.Category != null ? g.Category.Name : null,
                    CategorySlug = g.Category != null ? g.Category.Slug : null,
                    CurrentPrice = g.CurrentPrice,
                    OldPrice = g.OldPrice,
                    CreatedAt = g.CreatedAt,
                    CreatedByName = g.CreatedBy != null ? g.CreatedBy.FullName ?? g.CreatedBy.Email ?? "Unknown" : "Unknown",
                    IsFavorited = false
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GuideListViewModel>> GetPublishedGuidesAsync()
        {
            return await _context.Guides
                .Include(g => g.CreatedBy)
                .Include(g => g.Category)
                .Where(g => g.IsPublished)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new GuideListViewModel
                {
                    Id = g.Id,
                    Slug = g.Slug,
                    Title = g.Title,
                    Summary = g.Summary,
                    CoverImagePath = g.CoverImagePath,
                    IsPublished = g.IsPublished,
                    CategoryId = g.CategoryId,
                    CategoryName = g.Category != null ? g.Category.Name : null,
                    CategorySlug = g.Category != null ? g.Category.Slug : null,
                    CurrentPrice = g.CurrentPrice,
                    OldPrice = g.OldPrice,
                    CreatedAt = g.CreatedAt,
                    CreatedByName = g.CreatedBy != null ? g.CreatedBy.FullName ?? g.CreatedBy.Email ?? "Unknown" : "Unknown",
                    IsFavorited = false
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GuideListViewModel>> GetPublishedGuidesByCategoryAsync(string categorySlug, string? userId = null)
        {
            var query = _context.Guides
                .Include(g => g.CreatedBy)
                .Include(g => g.Category)
                .Where(g => g.IsPublished);

            if (!string.IsNullOrEmpty(categorySlug) && categorySlug.ToLower() != "all")
            {
                query = query.Where(g => g.Category != null && g.Category.Slug.ToLower() == categorySlug.ToLower());
            }

            var guides = await query
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new
                {
                    g.Id,
                    g.Slug,
                    g.Title,
                    g.Summary,
                    g.CoverImagePath,
                    g.IsPublished,
                    g.CategoryId,
                    CategoryName = g.Category != null ? g.Category.Name : null,
                    CategorySlug = g.Category != null ? g.Category.Slug : null,
                    g.CurrentPrice,
                    g.OldPrice,
                    g.CreatedAt,
                    CreatedByName = g.CreatedBy != null ? g.CreatedBy.FullName ?? g.CreatedBy.Email ?? "Unknown" : "Unknown"
                })
                .ToListAsync();

            // Get user favorites if userId is provided
            HashSet<int> favoritedGuideIds = new HashSet<int>();
            if (!string.IsNullOrEmpty(userId))
            {
                favoritedGuideIds = (await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.GuideId)
                    .ToListAsync())
                    .ToHashSet();
            }

            return guides.Select(g => new GuideListViewModel
            {
                Id = g.Id,
                Slug = g.Slug,
                Title = g.Title,
                Summary = g.Summary,
                CoverImagePath = g.CoverImagePath,
                IsPublished = g.IsPublished,
                CategoryId = g.CategoryId,
                CategoryName = g.CategoryName,
                CategorySlug = g.CategorySlug,
                CurrentPrice = g.CurrentPrice,
                OldPrice = g.OldPrice,
                CreatedAt = g.CreatedAt,
                CreatedByName = g.CreatedByName,
                IsFavorited = favoritedGuideIds.Contains(g.Id)
            }).ToList();
        }

        public async Task<IEnumerable<GuideListViewModel>> GetPublishedGuidesWithFavoritesAsync(string userId)
        {
            var guides = await _context.Guides
                .Include(g => g.CreatedBy)
                .Include(g => g.Category)
                .Where(g => g.IsPublished)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new
                {
                    g.Id,
                    g.Slug,
                    g.Title,
                    g.Summary,
                    g.CoverImagePath,
                    g.IsPublished,
                    g.CategoryId,
                    CategoryName = g.Category != null ? g.Category.Name : null,
                    CategorySlug = g.Category != null ? g.Category.Slug : null,
                    g.CurrentPrice,
                    g.OldPrice,
                    g.CreatedAt,
                    CreatedByName = g.CreatedBy != null ? g.CreatedBy.FullName ?? g.CreatedBy.Email ?? "Unknown" : "Unknown"
                })
                .ToListAsync();

            // Get user favorites
            var favoritedGuideIds = (await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.GuideId)
                .ToListAsync())
                .ToHashSet();

            return guides.Select(g => new GuideListViewModel
            {
                Id = g.Id,
                Slug = g.Slug,
                Title = g.Title,
                Summary = g.Summary,
                CoverImagePath = g.CoverImagePath,
                IsPublished = g.IsPublished,
                CategoryId = g.CategoryId,
                CategoryName = g.CategoryName,
                CategorySlug = g.CategorySlug,
                CurrentPrice = g.CurrentPrice,
                OldPrice = g.OldPrice,
                CreatedAt = g.CreatedAt,
                CreatedByName = g.CreatedByName,
                IsFavorited = favoritedGuideIds.Contains(g.Id)
            }).ToList();
        }

        public async Task<GuideDetailViewModel?> GetGuideByIdAsync(int id)
        {
            var guide = await _context.Guides
                .Include(g => g.CreatedBy)
                .Include(g => g.Category)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guide == null)
                return null;

            return new GuideDetailViewModel
            {
                Id = guide.Id,
                Slug = guide.Slug,
                Title = guide.Title,
                Summary = guide.Summary,
                CoverImagePath = guide.CoverImagePath,
                PdfPath = guide.PdfPath,
                CategoryId = guide.CategoryId,
                CategoryName = guide.Category?.Name,
                CurrentPrice = guide.CurrentPrice,
                OldPrice = guide.OldPrice,
                IsPublished = guide.IsPublished,
                CreatedAt = guide.CreatedAt,
                UpdatedAt = guide.UpdatedAt,
                CreatedByName = guide.CreatedBy?.FullName ?? guide.CreatedBy?.Email ?? "Unknown",
                CreatedByEmail = guide.CreatedBy?.Email ?? "Unknown"
            };
        }

        public async Task<GuideDetailViewModel?> GetGuideBySlugAsync(string slug)
        {
            var guide = await _context.Guides
                .Include(g => g.CreatedBy)
                .Include(g => g.Category)
                .FirstOrDefaultAsync(g => g.Slug == slug);

            if (guide == null)
                return null;

            return new GuideDetailViewModel
            {
                Id = guide.Id,
                Slug = guide.Slug,
                Title = guide.Title,
                Summary = guide.Summary,
                CoverImagePath = guide.CoverImagePath,
                PdfPath = guide.PdfPath,
                CategoryId = guide.CategoryId,
                CategoryName = guide.Category?.Name,
                CurrentPrice = guide.CurrentPrice,
                OldPrice = guide.OldPrice,
                IsPublished = guide.IsPublished,
                CreatedAt = guide.CreatedAt,
                UpdatedAt = guide.UpdatedAt,
                CreatedByName = guide.CreatedBy?.FullName ?? guide.CreatedBy?.Email ?? "Unknown",
                CreatedByEmail = guide.CreatedBy?.Email ?? "Unknown"
            };
        }

        public async Task<Guide?> CreateGuideAsync(GuideCreateViewModel model, string userId)
        {
            try
            {
                // Upload files
                var coverPath = await _fileService.UploadCoverAsync(model.CoverImage);
                var pdfPath = await _fileService.UploadPdfAsync(model.PdfFile);

                // Generate unique slug
                var slug = GenerateSlug(model.Title);
                var finalSlug = slug;
                int counter = 1;
                while (await SlugExistsAsync(finalSlug))
                {
                    finalSlug = $"{slug}-{counter}";
                    counter++;
                }

                var guide = new Guide
                {
                    Slug = finalSlug,
                    Title = model.Title,
                    Summary = model.Summary,
                    CoverImagePath = coverPath,
                    PdfPath = pdfPath,
                    CategoryId = model.CategoryId,
                    CurrentPrice = model.CurrentPrice,
                    OldPrice = model.OldPrice,
                    IsPublished = model.IsPublished,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Guides.Add(guide);
                await _context.SaveChangesAsync();

                await _logService.LogInfoAsync($"Guide created: {guide.Title} (ID: {guide.Id})", "GuideService");
                return guide;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error creating guide: {model.Title}", ex.Message, "GuideService");
                throw;
            }
        }

        public async Task<bool> UpdateGuideAsync(int id, GuideEditViewModel model)
        {
            try
            {
                var guide = await _context.Guides.FindAsync(id);
                if (guide == null)
                    return false;

                // Update cover image if provided
                if (model.CoverImage != null)
                {
                    await _fileService.DeleteFileAsync(guide.CoverImagePath);
                    guide.CoverImagePath = await _fileService.UploadCoverAsync(model.CoverImage);
                }

                // Update PDF if provided
                if (model.PdfFile != null)
                {
                    await _fileService.DeleteFileAsync(guide.PdfPath);
                    guide.PdfPath = await _fileService.UploadPdfAsync(model.PdfFile);
                }

                // Update other properties
                guide.Title = model.Title;
                guide.Summary = model.Summary;
                guide.CategoryId = model.CategoryId;
                guide.CurrentPrice = model.CurrentPrice;
                guide.OldPrice = model.OldPrice;
                guide.IsPublished = model.IsPublished;
                guide.UpdatedAt = DateTime.UtcNow;

                // Regenerate slug if title changed
                var newSlug = GenerateSlug(model.Title);
                if (newSlug != guide.Slug && !await SlugExistsAsync(newSlug, id))
                {
                    guide.Slug = newSlug;
                }

                await _context.SaveChangesAsync();
                await _logService.LogInfoAsync($"Guide updated: {guide.Title} (ID: {guide.Id})", "GuideService");
                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error updating guide ID: {id}", ex.Message, "GuideService");
                return false;
            }
        }

        public async Task<bool> DeleteGuideAsync(int id)
        {
            try
            {
                var guide = await _context.Guides.FindAsync(id);
                if (guide == null)
                    return false;

                // Delete associated files
                await _fileService.DeleteFileAsync(guide.CoverImagePath);
                await _fileService.DeleteFileAsync(guide.PdfPath);

                _context.Guides.Remove(guide);
                await _context.SaveChangesAsync();

                await _logService.LogInfoAsync($"Guide deleted: {guide.Title} (ID: {guide.Id})", "GuideService");
                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error deleting guide ID: {id}", ex.Message, "GuideService");
                return false;
            }
        }

        public async Task<bool> TogglePublishStatusAsync(int id)
        {
            var guide = await _context.Guides.FindAsync(id);
            if (guide == null)
                return false;

            guide.IsPublished = !guide.IsPublished;
            guide.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _logService.LogInfoAsync($"Guide publish status toggled: {guide.Title} (ID: {guide.Id}), Published: {guide.IsPublished}", "GuideService");
            return true;
        }

        public async Task<bool> GuideExistsAsync(int id)
        {
            return await _context.Guides.AnyAsync(g => g.Id == id);
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Guides.Where(g => g.Slug == slug);
            
            if (excludeId.HasValue)
            {
                query = query.Where(g => g.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "untitled";

            // Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove consecutive hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            // Limit length
            if (slug.Length > 200)
                slug = slug.Substring(0, 200).TrimEnd('-');

            return slug;
        }
    }
}

