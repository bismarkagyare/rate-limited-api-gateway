using Gateway.Api.Services.Interfaces;

namespace Gateway.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IRateLimitService _rateLimitService;

    public RateLimitingMiddleware(RequestDelegate next, IRateLimitService rateLimitService)
    {
        _next = next;
        _rateLimitService = rateLimitService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var apiKey = context.Request.Headers["X-API-Key"].ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            await _next(context);
            return;
        }

        var isAllowed = _rateLimitService.IsRequestAllowed(apiKey);

        //if not allowed, block the request
        if (!isAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }

        await _next(context);
    }
}
