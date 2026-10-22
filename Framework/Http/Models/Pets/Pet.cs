using Petstore.Reqnroll.Api.Tests.Framework.Http.Enums;

namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;

public record Pet
{
    public long Id { get; init; }
    public Category? Category { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<string> PhotoUrls { get; init; } = [];
    public List<Tag>? Tags { get; init; }
    public PetStatus? Status { get; init; }
}
