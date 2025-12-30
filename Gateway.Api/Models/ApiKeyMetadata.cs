namespace Gateway.Api.Models;

public class ApiKeyMetadata
{
    public string ApiKey { get; set; } = string.Empty;

    public string Plan { get; set; } = string.Empty;

    public int MaxRequests { get; set; }

    public int WindowSeconds { get; set; }

    public bool IsActive { get; set; }
}
