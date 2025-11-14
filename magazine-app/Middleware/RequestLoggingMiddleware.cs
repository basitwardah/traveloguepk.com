using System.Diagnostics;
using magazine_app.Services.Interfaces;

namespace magazine_app.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ILogService logService, IActivityService activityService)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            try
            {
                // Continue with the request
                await _next(context);

                stopwatch.Stop();

                // Log request details
                var logMessage = $"{requestMethod} {requestPath} - Status: {context.Response.StatusCode} - Duration: {stopwatch.ElapsedMilliseconds}ms - IP: {ipAddress}";
                _logger.LogInformation(logMessage);

                // Save to database (only for authenticated users)
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var action = $"{requestMethod} {requestPath}";
                        await activityService.LogActivityAsync(userId, action, null, ipAddress, userAgent);
                    }
                }

                // Log to database for specific status codes
                if (context.Response.StatusCode >= 400)
                {
                    await logService.LogWarningAsync(
                        $"HTTP {context.Response.StatusCode}: {requestMethod} {requestPath}",
                        "RequestLoggingMiddleware"
                    );
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log error
                _logger.LogError(ex, $"Error processing request: {requestMethod} {requestPath}");
                
                await logService.LogErrorAsync(
                    $"Request failed: {requestMethod} {requestPath}",
                    ex.ToString(),
                    "RequestLoggingMiddleware"
                );

                throw;
            }
        }
    }

    // Extension method to register the middleware
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}

