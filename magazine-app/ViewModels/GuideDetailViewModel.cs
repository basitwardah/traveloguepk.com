namespace magazine_app.ViewModels
{
    public class GuideDetailViewModel
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string CoverImagePath { get; set; } = string.Empty;
        public string PdfPath { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OldPrice { get; set; }
        public bool IsFree => CurrentPrice == 0;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string CreatedByEmail { get; set; } = string.Empty;
    }
}

