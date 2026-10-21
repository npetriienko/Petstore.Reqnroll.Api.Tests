namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;

public record Tag
{
    public long Id { get; init; }
    public string? Name { get; init; }
}