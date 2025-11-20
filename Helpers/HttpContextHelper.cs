using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
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

        string? request = await GetStringBodyAsync(httpContext.Request);
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

    private static async Task<string> GetStringBodyAsync(HttpRequest request)
    {
        string bodyContent = await GetStringBodyAsync(request.Body);

        if (bodyContent.Contains(HeaderNames.ContentDisposition))
        {
            return RemoveFileContent(request.Headers[HeaderNames.ContentType]!, bodyContent);
        }

        return bodyContent;
    }

    private static async Task<string> GetStringBodyAsync(Stream body)
    {
        using StreamReader bodyStream = new(body);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

        string stringContent = await bodyStream.ReadToEndAsync();

        body.Position = 0;

        return stringContent;
    }

    private static string RemoveFileContent(string contentTypeHeader, string bodyContent)
    {
        string boundary = contentTypeHeader.Substring(contentTypeHeader.IndexOf("boundary=") + 9);

        int contentDispositionIndex = bodyContent.IndexOf(HeaderNames.ContentDisposition);
        string contentDisposition = bodyContent.Substring(contentDispositionIndex, bodyContent.IndexOf("\r", contentDispositionIndex) - contentDispositionIndex);

        int contentTypeIndex = bodyContent.IndexOf(HeaderNames.ContentType);
        string contentType = bodyContent.Substring(contentTypeIndex, bodyContent.IndexOf("\r", contentTypeIndex) - contentTypeIndex);

        int closingBoundaryIndex = bodyContent.IndexOf(boundary, contentTypeIndex);
        int fileByteStart = contentTypeIndex + contentType.Length;
        int fileByteLength = closingBoundaryIndex - (contentTypeIndex + contentType.Length + 2);

        string fileContent = bodyContent.Substring(fileByteStart, fileByteLength).Trim();

        StringBuilder emptyFileUploadBody = new();
        emptyFileUploadBody.AppendLine(boundary);
        emptyFileUploadBody.AppendLine(contentDisposition);
        emptyFileUploadBody.AppendLine(contentType);
        emptyFileUploadBody.AppendLine($"<{fileContent.Length} bytes omitted>");
        emptyFileUploadBody.AppendLine();
        emptyFileUploadBody.AppendLine($"{boundary}--");

        return emptyFileUploadBody.ToString();
    }
}
