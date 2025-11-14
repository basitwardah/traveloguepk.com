using System.ComponentModel.DataAnnotations;

namespace magazine_app.Models
{
    public class Guide
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        [Required]
        [MaxLength(500)]
        public string CoverImagePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PdfPath { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = false;

        [Required]
        public string CreatedById { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ApplicationUser? CreatedBy { get; set; }
    }
}

