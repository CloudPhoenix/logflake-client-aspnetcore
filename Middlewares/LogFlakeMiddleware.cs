using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using NLogFlake.Extensions;
using NLogFlake.Helpers;
using NLogFlake.Models.Options;
using NLogFlake.Services;

namespace NLogFlake.Middlewares;

public class LogFlakeMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogFlakeService _logFlakeService;

    private readonly LogFlakeMiddlewareOptions _logFlakeMiddlewareOptions;

    private readonly LogFlakeMiddlewareSettingsOptions _logFlakeMiddlewareSettingsOptions;

    public LogFlakeMiddleware(RequestDelegate next, ILogFlakeService logFlakeService, IConfigureOptions<LogFlakeMiddlewareOptions> logFlakeMiddlewareOptions, IOptions<LogFlakeMiddlewareSettingsOptions> logFlakeMiddlewareSettingsOptions)
    {
        _next = next;

        _logFlakeService = logFlakeService ?? throw new ArgumentNullException(nameof(logFlakeService));
        _logFlakeMiddlewareOptions = new LogFlakeMiddlewareOptions();
        logFlakeMiddlewareOptions.Configure(_logFlakeMiddlewareOptions);

        _logFlakeMiddlewareSettingsOptions = logFlakeMiddlewareSettingsOptions.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext, ICorrelationService correlationService, IParameterService parameterService)
    {
        if (httpContext is null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        httpContext.Request.EnableBuffering();

        string fullPath = httpContext.Request.GetDisplayUrl();

        IPerformanceCounter? performance = null;

        bool ignoreLogProcessing = _logFlakeMiddlewareOptions.IgnoreLogProcessing(httpContext);
        if (!ignoreLogProcessing)
        {
            performance = _logFlakeService.MeasurePerformance();
        }

        Stream originalResponseStream = null!;
        using MemoryStream memoryStream = new();
        originalResponseStream = httpContext.Response.Body;
        httpContext.Response.Body = memoryStream;

        if (_logFlakeMiddlewareOptions.GlobalExceptionHandler)
        {
            await tryNextAsync(httpContext, ignoreLogProcessing, correlationService.Correlation);
        }
        else
        {
            await _next(httpContext);
        }

        string? endpointForPerformance = _logFlakeMiddlewareOptions.GetPerformanceMonitorLabel(httpContext);
        if (string.IsNullOrWhiteSpace(endpointForPerformance))
        {
            endpointForPerformance = fullPath;
        }

        performance?.SetLabel(endpointForPerformance);

        if (NotExistingEndpoint(httpContext) && !_logFlakeMiddlewareSettingsOptions.LogNotFoundErrors)
        {
            return;
        }

        string? response = null;
        if (_logFlakeMiddlewareSettingsOptions.LogResponse)
        {
            memoryStream.Position = 0;
            response = new StreamReader(memoryStream).ReadToEnd();
        }

        LogLevels level = LogLevels.DEBUG;
        switch (httpContext.Response.StatusCode)
        {
            case >= 500:
                level = LogLevels.ERROR;
                break;
            case >= 400:
                level = LogLevels.WARN;
                break;
            case >= 300:
                level = LogLevels.INFO;
                break;
        }

        if (httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest && EmptyResponseBody(httpContext))
        {
            await _logFlakeMiddlewareOptions.OnError(httpContext);
        }

        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalResponseStream);

        if (_logFlakeMiddlewareSettingsOptions.LogRequest && !ignoreLogProcessing)
        {
            string logMessage = $"{httpContext.Request.Method} {httpContext.Request.Path} status {httpContext.Response.StatusCode} in {performance!.Stop():N0} ms";

            Dictionary<string, object> content = await HttpContextHelper.GetLogParametersAsync(httpContext, response);

            if (!parameterService.IsEmpty())
            {
                content = content.Merge(parameterService.Get());
            }

            _logFlakeService.WriteLog(level, logMessage, correlationService.Correlation, content);
        }
    }

    private async Task tryNextAsync(HttpContext httpContext, bool ignoreLogProcessing, string correlation)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (!ignoreLogProcessing)
            {
                _logFlakeService.WriteException(ex, correlation);
            }

            if (!httpContext.Response.HasStarted)
            {
                await _logFlakeMiddlewareOptions.OnError(httpContext);
            }
        }
    }

    private static bool NotExistingEndpoint(HttpContext httpContext) => httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.Headers.Any();

    private static bool EmptyResponseBody(HttpContext httpContext) => httpContext.Response.Body is null || httpContext.Response.Body.Length == 0;
}
