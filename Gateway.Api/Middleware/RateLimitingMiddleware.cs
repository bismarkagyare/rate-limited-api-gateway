using System.Collections.Concurrent;
using Gateway.Api.Models;

namespace Gateway.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly ConcurrentDictionary<string, RateLimitEntry> _rateLimits = new();

    private const int MaxRequests = 5;

    private static readonly TimeSpan WindowDuration = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var apiKey = context.Request.Headers["X-API-Key"].ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            await _next(context);
            return;
        }

        var now = DateTime.UtcNow;

        var entry = _rateLimits.GetOrAdd(
            apiKey,
            _ => new RateLimitEntry { RequestCount = 0, WindowStart = now }
        );

        //check if the current window has expired and reset
        if (now - entry.WindowStart > WindowDuration)
        {
            entry.RequestCount = 0;
            entry.WindowStart = now;
        }

        entry.RequestCount++;

        //in case its exceeded
        if (entry.RequestCount > MaxRequests)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later");
            return;
        }

        await _next(context);
    }
}
