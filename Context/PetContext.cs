using System.Text.Json;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;
using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Context;

public sealed class PetContext
{
    public Pet? Pet { get; set; }
    
    public RestResponse? LastResponse { get; set; }

    // Ids of pets created on the server, deleted after the scenario
    public HashSet<long> CreatedPetIds { get; } = [];

    public Pet RequiredPet => Pet ?? throw new InvalidOperationException("No pet in context. Add a Given step that creates one.");

    public RestResponse RequiredLastResponse =>
        LastResponse ?? throw new InvalidOperationException("No response in context. Add a When step that calls the API.");
    
    public T? ResponseBodyAs<T>()
    {
        var content = RequiredLastResponse.Content;
        return string.IsNullOrEmpty(content) ? default : JsonSerializer.Deserialize<T>(content, PetstoreClient.JsonOptions);
    }
}
