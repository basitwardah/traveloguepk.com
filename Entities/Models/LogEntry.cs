using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = "Info";

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? Exception { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? Source { get; set; }
    }
}

