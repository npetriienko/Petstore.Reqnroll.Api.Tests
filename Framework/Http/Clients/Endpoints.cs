namespace Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;

public static class Endpoints
{
    /// <summary>
    /// POST | PUT a pet
    /// </summary>
    public static string Pet => "pet";

    /// <summary>
    /// GET | POST | DELETE pet by ID
    /// </summary>
    public static string PetById(long petId) => $"pet/{petId}";
}
