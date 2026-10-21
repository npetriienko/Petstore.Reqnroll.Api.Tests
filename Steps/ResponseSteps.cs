using FluentAssertions;
using Petstore.Reqnroll.Api.Tests.Context;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models;

namespace Petstore.Reqnroll.Api.Tests.Steps;

/// <summary>Then steps that check the last response, whatever endpoint produced it.</summary>
[Binding]
public sealed class ResponseSteps(PetContext context)
{
    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        var response = context.RequiredLastResponse;
        ((int)response.StatusCode).Should().Be(statusCode, "the response body was: {0}", response.Content);
    }

    [Then("the error response should have code {int}, type {string} and message {string}")]
    public void ThenTheErrorResponseShouldHave(int code, string type, string message) =>
        context.ResponseBodyAs<ApiResponse>().Should().BeEquivalentTo(new ApiResponse { Code = code, Type = type, Message = message });

    [Then("the response body should be empty")]
    public void ThenTheResponseBodyShouldBeEmpty() =>
        context.RequiredLastResponse.Content.Should().BeNullOrEmpty();
}
