using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using static NLogFlake.Constants.LogParameterConstants;

namespace NLogFlake.Helpers;

internal static class HttpContextHelper
{
    internal static async Task<Dictionary<string, object>> GetLogParametersAsync(HttpContext httpContext, string? clientIdSelector, string? response)
    {
        Dictionary<string, object> logParameters = new()
        {
            { RequestUriKey, new Uri(httpContext.Request.GetDisplayUrl()) },
            { RequestMethodKey, httpContext.Request.Method },
            { RequestHeadersKey, httpContext.Request.Headers },
        };

        string? request = await GetStringBodyAsync(httpContext.Request.Body);
        if (!string.IsNullOrWhiteSpace(request))
        {
            logParameters.Add(RequestBodyKey, request);
        }

        if (!string.IsNullOrEmpty(clientIdSelector) && httpContext.User is not null)
        {
            Claim clientIdClaim = httpContext.User.FindFirst(clientIdSelector);

            if (clientIdClaim is not null && !string.IsNullOrWhiteSpace(clientIdClaim.Value))
            {
                logParameters.Add(ClientIdKey, clientIdClaim.Value);
            }
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
