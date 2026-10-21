namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Models;

/// <summary>Generic message body the Petstore API returns for many operations.</summary>
public record ApiResponse
{
    public int Code { get; init; }
    public string? Type { get; init; }
    public string? Message { get; init; }
}
