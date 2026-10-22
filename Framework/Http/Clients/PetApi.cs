using Petstore.Reqnroll.Api.Tests.Framework.Http.Models;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;
using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;

public sealed class PetApi(RestClient client) : ApiClientBase(client)
{
    public Task<RestResponse<Pet>> AddPetAsync(Pet pet)
        => PostAsync<Pet>(Endpoints.Pet, pet);

    public Task<RestResponse<Pet>> GetPetByIdAsync(long petId)
        => GetAsync<Pet>(Endpoints.PetById(petId));

    public Task<RestResponse<ApiResponse>> DeletePetByIdAsync(long petId)
        => DeleteAsync<ApiResponse>(Endpoints.PetById(petId));
}
