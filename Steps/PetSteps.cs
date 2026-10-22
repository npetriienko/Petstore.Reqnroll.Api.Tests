using System.Net;
using FluentAssertions;
using Petstore.Reqnroll.Api.Tests.Context;
using Petstore.Reqnroll.Api.Tests.Framework.Data;
using Petstore.Reqnroll.Api.Tests.Framework.Http;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;
using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Steps;

[Binding]
public sealed class PetSteps(PetApi petApi, PetContext context)
{
    #region Given

    [Given("a valid pet with all fields")]
    public void GivenAValidPetWithAllFields() =>
        context.Pet = new PetBuilder().Build();

    [Given("a valid pet with only the required fields")]
    public void GivenAValidPetWithOnlyTheRequiredFields() =>
        context.Pet = new PetBuilder()
            .WithCategory(null)
            .WithTags(null)
            .WithStatus(null)
            .Build();

    [Given("a pet that does not exist")]
    public void GivenAPetThatDoesNotExist() =>
        // Built but never sent: a random id from a 9e15 range is not on the server.
        context.Pet = new PetBuilder().Build();

    [Given("an existing pet")]
    public async Task GivenAnExistingPet()
    {
        var response = await AddPetAsync(new PetBuilder().Build());
        response.StatusCode.Should().Be(HttpStatusCode.OK, "the pet must be created before the scenario starts");
    }

    #endregion

    #region When

    [When("I add the pet")]
    public Task WhenIAddThePet() =>
        AddPetAsync(context.RequiredPet);

    [When("I get the pet by its id")]
    public async Task WhenIGetThePetByItsId() =>
        context.LastResponse = await petApi.GetPetByIdAsync(context.RequiredPet.Id);

    [When("I delete the pet")]
    public async Task WhenIDeleteThePet()
    {
        var petId = context.RequiredPet.Id;
        var response = await petApi.DeletePetByIdAsync(petId);
        if (response.StatusCode == HttpStatusCode.OK)
            context.CreatedPetIds.Remove(petId);

        context.LastResponse = response;
    }

    #endregion

    #region Then

    [Then("the response pet should match the request")]
    public void ThenTheResponsePetShouldMatchTheRequest() =>
        context.RequiredLastResponse.BodyAs<Pet>()
            .Should().BeEquivalentTo(Expected(context.RequiredPet), o => o.ComparingByMembers<Pet>());

    [Then("the pet should not be retrievable by its id")]
    public async Task ThenThePetShouldNotBeRetrievableByItsId()
    {
        // Checked with its own request, so LastResponse still holds the response of the When step
        var getResponse = await petApi.GetPetByIdAsync(context.RequiredPet.Id);

        getResponse.StatusCode
            .Should().Be(HttpStatusCode.NotFound, "the pet should be gone, but GET returned: {0}", getResponse.Content);
    }

    [Then("the response message should be the pet id")]
    public void ThenTheResponseMessageShouldBeThePetId()
    {
        var body = context.RequiredLastResponse.BodyAs<ApiResponse>();
        body.Should().NotBeNull("the response should have a message body");
        body!.Message.Should().Be(context.RequiredPet.Id.ToString());
    }

    #endregion

    // Registers the pet for cleanup before sending, so it is deleted even if the call fails half-way
    private async Task<RestResponse> AddPetAsync(Pet pet)
    {
        context.Pet = pet;
        context.CreatedPetIds.Add(pet.Id);
        context.LastResponse = await petApi.AddPetAsync(pet);
        return context.LastResponse;
    }

    // The server returns tags: [] when a pet is sent without tags
    private static Pet Expected(Pet sent) => sent with { Tags = sent.Tags ?? [] };
}
