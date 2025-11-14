using Microsoft.AspNetCore.Identity;

namespace magazine_app.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Guide> CreatedGuides { get; set; } = new List<Guide>();
        public virtual ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();
    }
}

