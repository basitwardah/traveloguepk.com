using Microsoft.EntityFrameworkCore;
using magazine_app.Data;
using magazine_app.Models;
using magazine_app.Services.Interfaces;

namespace magazine_app.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string level, string message, string? exception = null, string? source = null)
        {
            var logEntry = new LogEntry
            {
                Level = level,
                Message = message,
                Exception = exception,
                Source = source,
                CreatedAt = DateTime.UtcNow
            };

            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();
        }

        public async Task LogInfoAsync(string message, string? source = null)
        {
            await LogAsync("Info", message, null, source);
        }

        public async Task LogWarningAsync(string message, string? source = null)
        {
            await LogAsync("Warning", message, null, source);
        }

        public async Task LogErrorAsync(string message, string? exception = null, string? source = null)
        {
            await LogAsync("Error", message, exception, source);
        }

        public async Task<IEnumerable<LogEntry>> GetLogsAsync(int count = 100, string? level = null)
        {
            var query = _context.LogEntries.AsQueryable();

            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(l => l.Level == level);
            }

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByDateRangeAsync(DateTime from, DateTime to, string? level = null)
        {
            var query = _context.LogEntries
                .Where(l => l.CreatedAt >= from && l.CreatedAt <= to);

            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(l => l.Level == level);
            }

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task ClearOldLogsAsync(int daysToKeep = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var oldLogs = await _context.LogEntries
                .Where(l => l.CreatedAt < cutoffDate)
                .ToListAsync();

            _context.LogEntries.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();
        }
    }
}

