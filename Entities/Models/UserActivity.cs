using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class UserActivity
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        public int? GuideId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Guide? Guide { get; set; }
    }
}

