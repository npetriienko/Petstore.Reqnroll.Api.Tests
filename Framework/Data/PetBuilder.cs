using Bogus;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Enums;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Models.Pets;

namespace Petstore.Reqnroll.Api.Tests.Framework.Data;

/// <summary>
/// Builder class for creating instances of <see cref="Pet"/> with customizable properties.
/// </summary>
public class PetBuilder
{
    // The public server is shared, and it turns id 0 into long.MaxValue, so every pet gets a random id.
    // The range stays below 2^53 so the id survives any JSON tooling unchanged.
    private const long MinId = 1_000_000_000_000;
    private const long MaxId = 9_000_000_000_000_000;

    private static readonly string[] CategoryNames = ["Dogs", "Cats", "Birds", "Fish", "Reptiles"];

    private readonly Faker<Pet> _petFaker;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetBuilder"/> class with all fields set to random valid values.
    /// </summary>
    public PetBuilder()
    {
        _petFaker = new Faker<Pet>()
            .RuleFor(p => p.Id, f => f.Random.Long(MinId, MaxId))
            .RuleFor(p => p.Name, f => f.Name.FirstName())
            .RuleFor(p => p.Category, f => new Category { Id = f.Random.Long(1, 1000), Name = f.PickRandom(CategoryNames) })
            .RuleFor(p => p.PhotoUrls, f => f.Make(f.Random.Int(1, 3), () => f.Image.PicsumUrl()).ToList())
            .RuleFor(p => p.Tags, f => f.Make(f.Random.Int(1, 3), () => new Tag { Id = f.Random.Long(1, 1000), Name = f.Lorem.Word() }).ToList())
            .RuleFor(p => p.Status, f => f.PickRandom<PetStatus>());
    }

    /// <summary>
    /// Sets the id of the pet.
    /// </summary>
    /// <param name="id">The id to set.</param>
    /// <returns>The current instance of <see cref="PetBuilder"/>.</returns>
    public PetBuilder WithId(long id)
    {
        _petFaker.RuleFor(p => p.Id, _ => id);
        return this;
    }

    /// <summary>
    /// Sets the name of the pet.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current instance of <see cref="PetBuilder"/>.</returns>
    public PetBuilder WithName(string name)
    {
        _petFaker.RuleFor(p => p.Name, _ => name);
        return this;
    }

    /// <summary>
    /// Sets the category of the pet.
    /// </summary>
    /// <param name="category">The category to set, or <c>null</c> to leave it out.</param>
    /// <returns>The current instance of <see cref="PetBuilder"/>.</returns>
    public PetBuilder WithCategory(Category? category)
    {
        _petFaker.RuleFor(p => p.Category, _ => category);
        return this;
    }

    /// <summary>
    /// Sets the photo URLs of the pet.
    /// </summary>
    /// <param name="photoUrls">The photo URLs to set.</param>
    /// <returns>The current instance of <see cref="PetBuilder"/>.</returns>
    public PetBuilder WithPhotoUrls(List<string> photoUrls)
    {
        _petFaker.RuleFor(p => p.PhotoUrls, _ => photoUrls);
        return this;
    }

    /// <summary>
    /// Sets the tags of the pet.
    /// </summary>
    /// <param name="tags">The tags to set, or <c>null</c> to leave them out.</param>
    /// <returns>The current instance of <see cref="PetBuilder"/>.</returns>
    public PetBuilder WithTags(List<Tag>? tags)
    {
        _petFaker.RuleFor(p => p.Tags, _ => tags);
        return this;
    }

    /// <summary>
    /// Sets the status of the pet.
    /// </summary>
    /// <param name="status">The status to set, or <c>null</c> to leave it out.</param>
    /// <returns>The current instance of <see cref="PetBuilder"/>.</returns>
    public PetBuilder WithStatus(PetStatus? status)
    {
        _petFaker.RuleFor(p => p.Status, _ => status);
        return this;
    }

    /// <summary>
    /// Builds and returns an instance of <see cref="Pet"/> with the specified properties.
    /// </summary>
    /// <returns>An instance of <see cref="Pet"/>.</returns>
    public Pet Build()
    {
        return _petFaker.Generate();
    }
}
