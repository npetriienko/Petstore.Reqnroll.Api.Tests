using Microsoft.Extensions.Configuration;

namespace Petstore.Reqnroll.Api.Tests.Framework.Config;

public sealed class ClientSettings
{
    private const string SectionName = "ClientSettings";
    private const string ConfigFileName = "config.json";

    public string BaseUrl { get; init; } = string.Empty;
    public int TimeoutSeconds { get; init; } = 30;

    public static ClientSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(ConfigFileName, optional: false)
            .AddEnvironmentVariables()
            .Build();

        var settings = configuration.GetSection(SectionName).Get<ClientSettings>()
                       ?? throw new ConfigurationException($"Section '{SectionName}' is missing in '{ConfigFileName}'");

        settings.Validate();
        return settings;
    }

    private void Validate()
    {
        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
            throw new ConfigurationException($"{SectionName}:BaseUrl must be an absolute URL, but was '{BaseUrl}'");

        if (TimeoutSeconds <= 0)
            throw new ConfigurationException($"{SectionName}:TimeoutSeconds must be positive, but was {TimeoutSeconds}");
    }
}
