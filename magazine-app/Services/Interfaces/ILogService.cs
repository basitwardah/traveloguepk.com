using magazine_app.Models;

namespace magazine_app.Services.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(string level, string message, string? exception = null, string? source = null);
        Task LogInfoAsync(string message, string? source = null);
        Task LogWarningAsync(string message, string? source = null);
        Task LogErrorAsync(string message, string? exception = null, string? source = null);
        Task<IEnumerable<LogEntry>> GetLogsAsync(int count = 100, string? level = null);
        Task<IEnumerable<LogEntry>> GetLogsByDateRangeAsync(DateTime from, DateTime to, string? level = null);
        Task ClearOldLogsAsync(int daysToKeep = 30);
    }
}

