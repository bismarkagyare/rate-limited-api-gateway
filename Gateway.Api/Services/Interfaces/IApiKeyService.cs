using Gateway.Api.Models;

namespace Gateway.Api.Services.Interfaces;

public interface IApiKeyService
{
    Task<ApiKeyMetadata?> GetApiKeyMetadataAsync(string apiKey);
}
