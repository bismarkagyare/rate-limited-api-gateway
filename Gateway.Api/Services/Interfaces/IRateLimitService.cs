using Gateway.Api.Models;

namespace Gateway.Api.Services.Interfaces;

public interface IRateLimitService
{
    RateLimitResult Evaluate(string apiKey);
}
