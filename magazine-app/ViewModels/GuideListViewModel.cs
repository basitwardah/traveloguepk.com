namespace magazine_app.ViewModels
{
    public class GuideListViewModel
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string CoverImagePath { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OldPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public bool IsFavorited { get; set; }
    }
}

