using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLogFlake.Models.Options;
using NLogFlake.Services;

namespace NLogFlake;

public static class IServiceCollectionExtensionsAspNet
{
    public static IServiceCollection ConfigureLogFlakeMiddlewareOptions(this IServiceCollection services, IConfiguration configuration, CorrelationType correlationType = CorrelationType.Guid)
    {
        services.TryAddScoped<IParameterService, ParameterService>();

        TryRegisterCorrelationService(services, correlationType);

        _ = services.Configure<LogFlakeMiddlewareSettingsOptions>(configuration.GetSection(LogFlakeMiddlewareSettingsOptions.SectionName));

        return services;
    }

    private static void TryRegisterCorrelationService(IServiceCollection services, CorrelationType correlationType)
    {
        if (correlationType == CorrelationType.Custom) return;

        if (correlationType == CorrelationType.Guid)
        {
            services.AddScoped<ICorrelationService, GuidCorrelationService>();
        }
        else if (correlationType == CorrelationType.TraceIdentifier)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICorrelationService, TraceIdentifierCorrelationService>();
        }
        else
        {
            throw new ArgumentException("CorrelationType not supported.", nameof(correlationType));
        }
    }
}
