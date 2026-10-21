using System.Text.Json;
using System.Text.Json.Serialization;
using Petstore.Reqnroll.Api.Tests.Framework.Config;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;

/// <summary>
/// Entry point to the Petstore API. Owns one configured <see cref="RestClient"/> shared by all controllers.
/// </summary>
public class PetstoreClient : IDisposable
{
    // camelCase JSON, enums as camelCase strings, nulls omitted from requests
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly RestClient _client;

    public PetController PetController { get; }

    public PetstoreClient(ClientSettings settings)
    {
        var options = new RestClientOptions(settings.BaseUrl)
        {
            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds),
            ThrowOnAnyError = false
        };

        _client = new RestClient(options, configureSerialization: s => s.UseSystemTextJson(JsonOptions));
        PetController = new PetController(_client);
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
