using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using NLogFlake.Extensions;

using static NLogFlake.Constants.LogParameterConstants;

namespace NLogFlake.Helpers;

internal static class HttpContextHelper
{
    internal static async Task<Dictionary<string, object>> GetLogParametersAsync(HttpContext httpContext, string? response)
    {
        Dictionary<string, object> logParameters = new()
        {
            { RequestUriKey, new Uri(httpContext.Request.GetDisplayUrl()) },
            { RequestMethodKey, httpContext.Request.Method },
            { RequestHeadersKey, httpContext.Request.Headers.MaskAuthorizationHeader() },
        };

        string? request = await GetStringBodyAsync(httpContext.Request.Body);
        if (!string.IsNullOrWhiteSpace(request))
        {
            logParameters.Add(RequestBodyKey, request);
        }

        if (!string.IsNullOrEmpty(response))
        {
            logParameters.Add(ResponseHeadersKey, httpContext.Response.Headers);
            logParameters.Add(ResponseStatusKey, httpContext.Response.StatusCode);

            if (!string.IsNullOrWhiteSpace(response))
            {
                logParameters.Add(ResponseBodyKey, response);
            }
        }

        return logParameters;
    }

    internal static async Task<string> GetStringBodyAsync(Stream body)
    {
        using StreamReader bodyStream = new(body);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

        string stringContent = await bodyStream.ReadToEndAsync();

        body.Position = 0;

        return stringContent;
    }
}
