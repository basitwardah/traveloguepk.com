using System.ComponentModel.DataAnnotations;

namespace magazine_app.ViewModels
{
    public class GuideEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Magazine Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Summary/Description")]
        [StringLength(1000, ErrorMessage = "Summary cannot exceed 1000 characters")]
        public string? Summary { get; set; }

        [Display(Name = "Cover Image (leave empty to keep existing)")]
        public IFormFile? CoverImage { get; set; }

        [Display(Name = "PDF Document (leave empty to keep existing)")]
        public IFormFile? PdfFile { get; set; }

        [Display(Name = "Publish")]
        public bool IsPublished { get; set; }

        // Existing file paths
        public string ExistingCoverImagePath { get; set; } = string.Empty;
        public string ExistingPdfPath { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}

