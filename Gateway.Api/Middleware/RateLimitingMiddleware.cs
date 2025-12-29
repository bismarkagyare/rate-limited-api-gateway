using Gateway.Api.Common.Constants;
using Gateway.Api.Common.Errors;
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
        var apiKey = context.Request.Headers[HeaderNames.ApiKey].ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            await _next(context);
            return;
        }

        var result = _rateLimitService.Evaluate(apiKey);

        //add rate limit headers to response
        context.Response.Headers[HeaderNames.RateLimitLimit] = result.Limit.ToString();
        context.Response.Headers[HeaderNames.RateLimitRemaining] = result.Remaining.ToString();
        context.Response.Headers[HeaderNames.RateLimitReset] = new DateTimeOffset(result.ResetTime)
            .ToUnixTimeSeconds()
            .ToString();

        //if not allowed, block the request
        if (!result.IsAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync(ErrorMessages.RateLimitExceeded);
            return;
        }

        await _next(context);
    }
}
