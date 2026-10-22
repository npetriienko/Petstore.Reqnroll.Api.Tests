using System.Text.Json;
using System.Text.Json.Serialization;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http;

// JSON settings shared by the API client
public static class JsonConfig
{
    // camelCase JSON, enums as camelCase strings, nulls omitted from requests
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
}
