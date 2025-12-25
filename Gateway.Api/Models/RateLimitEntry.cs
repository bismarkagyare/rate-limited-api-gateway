namespace Gateway.Api.Models;

public class RateLimitEntry
{
    public int RequestCount { get; set; }

    public DateTime WindowStart { get; set; }
}
