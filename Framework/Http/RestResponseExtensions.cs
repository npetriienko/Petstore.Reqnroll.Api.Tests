using System.Text.Json;
using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http;

public static class RestResponseExtensions
{
    // Reads the response body or returns default when the body is empty
    public static T? BodyAs<T>(this RestResponse response) =>
        string.IsNullOrEmpty(response.Content) ? default : JsonSerializer.Deserialize<T>(response.Content, JsonConfig.Options);
}
