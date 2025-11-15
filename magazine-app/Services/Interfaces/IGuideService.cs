using Entities.Models;
using magazine_app.ViewModels;

namespace magazine_app.Services.Interfaces
{
    public interface IGuideService
    {
        Task<IEnumerable<GuideListViewModel>> GetAllGuidesAsync();
        Task<IEnumerable<GuideListViewModel>> GetPublishedGuidesAsync();
        Task<IEnumerable<GuideListViewModel>> GetPublishedGuidesByCategoryAsync(string categorySlug, string? userId = null);
        Task<IEnumerable<GuideListViewModel>> GetPublishedGuidesWithFavoritesAsync(string userId);
        Task<GuideDetailViewModel?> GetGuideByIdAsync(int id);
        Task<GuideDetailViewModel?> GetGuideBySlugAsync(string slug);
        Task<Guide?> CreateGuideAsync(GuideCreateViewModel model, string userId);
        Task<bool> UpdateGuideAsync(int id, GuideEditViewModel model);
        Task<bool> DeleteGuideAsync(int id);
        Task<bool> TogglePublishStatusAsync(int id);
        Task<bool> GuideExistsAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        string GenerateSlug(string title);
    }
}

