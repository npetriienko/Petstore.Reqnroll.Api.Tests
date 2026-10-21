using Petstore.Reqnroll.Api.Tests.Framework.Http.Models;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;
using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;

public class PetController(RestClient client) : HttpRequests(client)
{
    public Task<RestResponse<Pet>> AddPetAsync(Pet petRequest)
        => PostAsync<Pet>(Endpoints.Pet, petRequest);

    public Task<RestResponse<Pet>> GetPetByIdAsync(long petId)
        => GetAsync<Pet>(Endpoints.PetById(petId));
    
    public Task<RestResponse<ApiResponse>> DeletePetByIdAsync(long petId)
        => DeleteAsync<ApiResponse>(Endpoints.PetById(petId));
}
