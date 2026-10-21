using System.Net;
using System.Text;
using System.Text.Json;
using Petstore.Reqnroll.Api.Tests.Context;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;
using RestSharp;

namespace Petstore.Reqnroll.Api.Tests.Hooks;

[Binding]
public sealed class PetHooks(
    PetContext context,
    PetController petController,
    ScenarioContext scenarioContext,
    IReqnrollOutputHelper output)
{
    /// <summary>Runs first, so the log shows the response that made the scenario fail, before cleanup calls.</summary>
    [AfterScenario(Order = 0)]
    public void LogLastExchangeOnFailure()
    {
        if (scenarioContext.TestError is null || context.Response is null)
            return;

        output.WriteLine(Describe(context.Response));
    }

    [AfterScenario(Order = 100)]
    public async Task DeleteCreatedPets()
    {
        foreach (var petId in context.CreatedPetIds)
        {
            var response = await petController.DeletePetByIdAsync(petId);
            if (response.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.NotFound))
                output.WriteLine($"Cleanup: deleting pet {petId} returned {(int)response.StatusCode}");
        }
    }

    private static string Describe(RestResponse response)
    {
        var request = response.Request;
        var body = request.Parameters.OfType<BodyParameter>().FirstOrDefault()?.Value switch
        {
            null => "<none>",
            string text => text,
            var value => JsonSerializer.Serialize(value, PetstoreClient.JsonOptions)
        };

        return new StringBuilder()
            .AppendLine("---- Last request ----")
            .AppendLine($"{request.Method.ToString().ToUpperInvariant()} {response.ResponseUri}")
            .AppendLine($"Body: {body}")
            .AppendLine("---- Response ----")
            .AppendLine($"Status: {(int)response.StatusCode} {response.StatusCode}")
            .AppendLine($"Body: {(string.IsNullOrEmpty(response.Content) ? "<empty>" : response.Content)}")
            .ToString();
    }
}
