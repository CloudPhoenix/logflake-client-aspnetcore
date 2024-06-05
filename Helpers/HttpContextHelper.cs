using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace NLogFlake.Helpers;

internal static class HttpContextHelper
{
    internal static async Task<Dictionary<string, object>> GetLogParametersAsync(HttpContext httpContext, bool includeResponse)
    {
        string? request = await GetStringBodyAsync(httpContext.Request.Body);
        Dictionary<string, object> exceptionParams = new()
        {
            {"requestUri", new Uri(httpContext.Request.GetDisplayUrl())},
            {"requestMethod", httpContext.Request.Method},
            {"requestHeaders", httpContext.Request.Headers},
        };

        if (!string.IsNullOrWhiteSpace(request))
        {
            exceptionParams.Add("requestBody", request);
        }

        if (includeResponse)
        {
            exceptionParams.Add("responseHeaders", httpContext.Response.Headers);
            exceptionParams.Add("responseStatus", httpContext.Response.StatusCode);

            string? response = await GetStringBodyAsync(httpContext.Response.Body);
            if (!string.IsNullOrWhiteSpace(response))
            {
                exceptionParams.Add("responseBody", response);
            }
        }

        return exceptionParams;
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
