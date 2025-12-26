using System.Collections.Concurrent;
using Gateway.Api.Models;
using Gateway.Api.Services.Interfaces;

namespace Gateway.Api.Services.Implementations;

public class RateLimitService : IRateLimitService
{
    private readonly ConcurrentDictionary<string, RateLimitEntry> _rateLimits = new();

    private const int MaxRequests = 5;

    private static readonly TimeSpan WindowDuration = TimeSpan.FromMinutes(1);

    public bool IsRequestAllowed(string apiKey)
    {
        var now = DateTime.UtcNow;

        var entry = _rateLimits.GetOrAdd(
            apiKey,
            _ => new RateLimitEntry { RequestCount = 0, WindowStart = now }
        );

        //check if the current window has expired and reset
        if (now - entry.WindowStart > WindowDuration)
        {
            entry.WindowStart = now;
            entry.RequestCount = 0;
        }

        entry.RequestCount++;

        //If limit exceeded, block request
        return entry.RequestCount <= MaxRequests;
    }
}
