using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int GuideId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Guide? Guide { get; set; }
    }
}

