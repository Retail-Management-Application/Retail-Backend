using System.Collections.Concurrent;

namespace RetailOrdering.API.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;

    // IP → (request count, window start)
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)>
        _clients = new();

    private const int MaxRequests = 100;
    private const int WindowSeconds = 60;

    public RateLimitMiddleware(RequestDelegate next,
                               ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;

        _clients.AddOrUpdate(ip,
            _ => (1, now),
            (_, existing) =>
            {
                if ((now - existing.WindowStart).TotalSeconds >= WindowSeconds)
                    return (1, now);
                return (existing.Count + 1, existing.WindowStart);
            });

        if (_clients.TryGetValue(ip, out var current) && current.Count > MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for IP: {IP}", ip);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please slow down.");
            return;
        }

        await _next(context);
    }
}
