using Microsoft.Extensions.DependencyInjection;
using Petstore.Reqnroll.Api.Tests.Context;
using Petstore.Reqnroll.Api.Tests.Framework.Config;
using Petstore.Reqnroll.Api.Tests.Framework.Http.Clients;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace Petstore.Reqnroll.Api.Tests.Hooks;

public static class DependencyRegistration
{
    // Step and hook classes are registered automatically
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        // Singletons live for the whole test run
        services.AddSingleton(_ => ClientSettings.Load());
        services.AddSingleton<PetstoreClient>();
        services.AddSingleton(sp => sp.GetRequiredService<PetstoreClient>().PetController);

        // Scoped services get a new instance per scenario
        services.AddScoped<PetContext>();

        return services;
    }
}
