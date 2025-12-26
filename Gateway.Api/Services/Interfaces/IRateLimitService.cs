namespace Gateway.Api.Services.Interfaces;

public interface IRateLimitService
{
    bool IsRequestAllowed(string apiKey);
}
