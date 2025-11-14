using System.ComponentModel.DataAnnotations;

namespace magazine_app.ViewModels
{
    public class GuideCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Magazine Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Summary/Description")]
        [StringLength(1000, ErrorMessage = "Summary cannot exceed 1000 characters")]
        public string? Summary { get; set; }

        [Required(ErrorMessage = "Cover image is required")]
        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; } = null!;

        [Required(ErrorMessage = "PDF file is required")]
        [Display(Name = "PDF Document")]
        public IFormFile PdfFile { get; set; } = null!;

        [Display(Name = "Publish Immediately")]
        public bool IsPublished { get; set; } = false;
    }
}

