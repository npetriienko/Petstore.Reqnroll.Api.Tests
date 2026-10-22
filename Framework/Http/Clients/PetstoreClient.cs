using Petstore.Reqnroll.Api.Tests.Framework.Config;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;

/// <summary>
/// Entry point to the Petstore API. Owns one configured <see cref="RestClient"/> shared by all API clients.
/// </summary>
public class PetstoreClient : IDisposable
{
    private readonly RestClient _client;

    public PetApi PetApi { get; }

    public PetstoreClient(ClientSettings settings)
    {
        var options = new RestClientOptions(settings.BaseUrl)
        {
            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds),
            ThrowOnAnyError = false
        };

        _client = new RestClient(options, configureSerialization: s => s.UseSystemTextJson(JsonConfig.Options));
        PetApi = new PetApi(_client);
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
