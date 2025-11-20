using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NLogFlake.Models.Options;
using NLogFlake.Services;

namespace NLogFlake;

public static class IHostApplicationBuilderExtensionsAspNet
{
    /// <summary>
    /// Configures LogFlake middleware options and registers required services for correlation and parameter handling.
    /// </summary>
    /// <remarks>This method registers the necessary services and configures options required for LogFlake
    /// middleware integration. It should be called during application startup as part of service
    /// configuration.
    /// <param name="services">The service collection to which the LogFlake middleware services and options will be added. Cannot be null.</param>
    /// <param name="configuration">The application configuration from which LogFlake middleware settings will be read. Cannot be null.</param>
    /// <param name="correlationType">The type of correlation identifier to use for request tracking. The default is CorrelationType.Guid.</param>
    /// <returns>The same IServiceCollection instance, enabling method chaining.</returns>
    public static IHostApplicationBuilder ConfigureLogFlakeMiddlewareOptions(this IHostApplicationBuilder builder, CorrelationType correlationType = CorrelationType.Guid)
    {
        IServiceCollection services = builder.Services;

        services.TryAddScoped<IParameterService, ParameterService>();

        TryRegisterCorrelationService(services, correlationType);

        _ = services.Configure<LogFlakeMiddlewareSettingsOptions>(builder.Configuration.GetSection(LogFlakeMiddlewareSettingsOptions.SectionName));

        return builder;
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
