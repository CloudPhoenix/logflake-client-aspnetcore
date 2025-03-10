using Microsoft.AspNetCore.Http;

namespace NLogFlake.Extensions;

internal static class HttpContextExtensions
{
    internal static bool NotExistingEndpoint(this HttpContext httpContext) => httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.Headers.Any();

    internal static bool EmptyResponseBody(this HttpContext httpContext) => httpContext.Response.Body is null || httpContext.Response.Body.Length == 0;
}
