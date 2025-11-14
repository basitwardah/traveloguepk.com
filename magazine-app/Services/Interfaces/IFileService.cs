namespace magazine_app.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadCoverAsync(IFormFile file);
        Task<string> UploadPdfAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string filePath);
        bool ValidateCoverImage(IFormFile file);
        bool ValidatePdf(IFormFile file);
        string GetFullPath(string relativePath);
        long MaxCoverSizeMB { get; }
        long MaxPdfSizeMB { get; }
    }
}

