using Gateway.Api.Common.Constants;
using Gateway.Api.Common.Errors;

namespace Gateway.Api.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    private const string validApiKey = "test-api-key";

    public ApiKeyAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderNames.ApiKey, out var apikey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(ErrorMessages.MissingApiKey);
            return;
        }

        if (apikey != validApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(ErrorMessages.InvalidApiKey);
            return;
        }

        await _next(context);
    }
}
