using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;

/// <summary>
/// Base class for API clients. Never throws on HTTP errors: the full response is returned
/// so tests can assert on status code, headers and body.
/// </summary>
public abstract class ApiClientBase(RestClient client)
{
    protected Task<RestResponse<TResult>> GetAsync<TResult>(string route)
    {
        var request = new RestRequest(route, Method.Get);

        return client.ExecuteAsync<TResult>(request);
    }

    protected Task<RestResponse<TResult>> PostAsync<TResult>(string route, object body)
    {
        var request = new RestRequest(route, Method.Post)
            .AddJsonBody(body);

        return client.ExecuteAsync<TResult>(request);
    }

    protected Task<RestResponse<TResult>> DeleteAsync<TResult>(string route)
    {
        var request = new RestRequest(route, Method.Delete);

        return client.ExecuteAsync<TResult>(request);
    }
}
