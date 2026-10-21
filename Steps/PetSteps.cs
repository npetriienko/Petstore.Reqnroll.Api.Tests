using System.Net;
using FluentAssertions;
using Petstore.Reqnroll.Api.Tests.Context;
using Petstore.Reqnroll.Api.Tests.Framework.Data;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;

namespace Petstore.Reqnroll.Api.Tests.Steps;

[Binding]
public sealed class PetSteps(PetController petController, PetContext context)
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

    [Given("a pet id that does not exist")]
    public void GivenAPetIdThatDoesNotExist() =>
        // Built but never sent: a random id from a 9e15 range is not on the server.
        context.Pet = new PetBuilder().Build();

    [Given("an existing pet")]
    public async Task GivenAnExistingPet()
    {
        var pet = new PetBuilder().Build();
        context.CreatedPetIds.Add(pet.Id);

        var created = await petController.AddPetAsync(pet);
        created.StatusCode.Should().Be(HttpStatusCode.OK, "the pet must be created before the scenario starts");

        context.Pet = pet;
    }

    #endregion

    #region When

    [When("I add the pet")]
    public async Task WhenIAddThePet()
    {
        var pet = context.RequiredPet;
        context.CreatedPetIds.Add(pet.Id);
        context.LastResponse = await petController.AddPetAsync(pet);
    }

    [When("I get the pet by its id")]
    public async Task WhenIGetThePetByItsId() =>
        context.LastResponse = await petController.GetPetByIdAsync(context.RequiredPet.Id);

    [When("I delete the pet")]
    public async Task WhenIDeleteThePet()
    {
        var petId = context.RequiredPet.Id;
        var response = await petController.DeletePetByIdAsync(petId);
        if (response.StatusCode == HttpStatusCode.OK)
            context.CreatedPetIds.Remove(petId);

        context.LastResponse = response;
    }

    #endregion

    #region Then

    [Then("the response pet should match the request")]
    public void ThenTheResponsePetShouldMatchTheRequest() =>
        context.ResponseBodyAs<Pet>().Should().BeEquivalentTo(Expected(context.RequiredPet), o => o.ComparingByMembers<Pet>());

    [Then("the pet should not be retrievable by its id")]
    public async Task ThenThePetShouldNotBeRetrievableByItsId()
    {
        context.LastResponse = await petController.GetPetByIdAsync(context.RequiredPet.Id);

        context.LastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then("the response message should be the pet id")]
    public void ThenTheResponseMessageShouldBeThePetId()
    {
        var body = context.ResponseBodyAs<ApiResponse>();
        body.Should().NotBeNull("the response should have a message body");
        body!.Message.Should().Be(context.RequiredPet.Id.ToString());
    }

    #endregion

    // The server returns tags: [] when a pet is sent without tags
    private static Pet Expected(Pet sent) => sent with { Tags = sent.Tags ?? [] };
}
