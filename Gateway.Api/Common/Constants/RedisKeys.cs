namespace Gateway.Api.Common.Constants;

public static class RedisKeys
{
    public static string RateLimit(string apiKey) => $"ratelimit:{apiKey}";
}
