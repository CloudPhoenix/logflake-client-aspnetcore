using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLogFlake.Models.Options;

namespace NLogFlake;

public static class IServiceCollectionExtensionsAspNet
{
    public static IServiceCollection AddLogFlake(this IServiceCollection services, IConfiguration configuration)
    {
        IServiceCollectionExtensions.AddLogFlake(services, configuration);

        _ = services.Configure<LogFlakeMiddlewareSettingsOptions>(configuration.GetSection(LogFlakeMiddlewareSettingsOptions.SectionName));

        return services;
    }
}
