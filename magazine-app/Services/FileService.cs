using magazine_app.Services.Interfaces;

namespace magazine_app.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogService _logService;
        private const string CoverUploadPath = "wwwroot/uploads/guides/covers";
        private const string PdfUploadPath = "wwwroot/uploads/guides/pdfs";
        private readonly string[] _allowedCoverExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] _allowedPdfExtensions = { ".pdf" };

        public long MaxCoverSizeMB => 5; // 5MB
        public long MaxPdfSizeMB => 50;  // 50MB

        public FileService(IWebHostEnvironment environment, ILogService logService)
        {
            _environment = environment;
            _logService = logService;
        }

        public async Task<string> UploadCoverAsync(IFormFile file)
        {
            if (!ValidateCoverImage(file))
            {
                throw new InvalidOperationException("Invalid cover image file.");
            }

            var uploadPath = Path.Combine(_environment.ContentRootPath, CoverUploadPath);
            Directory.CreateDirectory(uploadPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _logService.LogInfoAsync($"Cover image uploaded: {uniqueFileName}", "FileService");
            return $"/uploads/guides/covers/{uniqueFileName}";
        }

        public async Task<string> UploadPdfAsync(IFormFile file)
        {
            if (!ValidatePdf(file))
            {
                throw new InvalidOperationException("Invalid PDF file.");
            }

            var uploadPath = Path.Combine(_environment.ContentRootPath, PdfUploadPath);
            Directory.CreateDirectory(uploadPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _logService.LogInfoAsync($"PDF uploaded: {uniqueFileName}", "FileService");
            return $"/uploads/guides/pdfs/{uniqueFileName}";
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = GetFullPath(filePath);
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    await _logService.LogInfoAsync($"File deleted: {filePath}", "FileService");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Error deleting file: {filePath}", ex.Message, "FileService");
                return false;
            }
        }

        public bool ValidateCoverImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedCoverExtensions.Contains(extension))
                return false;

            if (file.Length > MaxCoverSizeMB * 1024 * 1024)
                return false;

            return true;
        }

        public bool ValidatePdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedPdfExtensions.Contains(extension))
                return false;

            if (file.Length > MaxPdfSizeMB * 1024 * 1024)
                return false;

            // Check MIME type
            if (file.ContentType != "application/pdf")
                return false;

            return true;
        }

        public string GetFullPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            // Remove leading slash if present
            relativePath = relativePath.TrimStart('/');
            
            return Path.Combine(_environment.ContentRootPath, "wwwroot", relativePath);
        }
    }
}

