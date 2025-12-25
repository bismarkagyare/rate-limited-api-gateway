namespace Gateway.Api.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    private const string ApiKeyHeaderName = "X-API-Key";

    private const string validApiKey = "test-api-key";

    public ApiKeyAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apikey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API key is missing");
            return;
        }

        if (apikey != validApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        await _next(context);
    }
}
