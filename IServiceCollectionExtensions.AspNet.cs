using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLogFlake.Models.Options;

namespace NLogFlake;

public static class IServiceCollectionExtensionsAspNet
{
    public static IServiceCollection ConfigureLogFlakeMiddlewareOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IParameterService, ParameterService>();
        _ = services.Configure<LogFlakeMiddlewareSettingsOptions>(configuration.GetSection(LogFlakeMiddlewareSettingsOptions.SectionName));

        return services;
    }
}
