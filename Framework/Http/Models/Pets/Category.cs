namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;

public record Category
{
    public long Id { get; init; }
    public string? Name { get; init; }
}