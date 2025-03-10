using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace NLogFlake.Extensions;

internal static class IHeaderDictionaryExtensions
{
    internal static IHeaderDictionary MaskAuthorizationHeader(this IHeaderDictionary headers)
    {
        if (headers.All(h => h.Key != HeaderNames.Authorization)) return headers;

        string auth = headers[HeaderNames.Authorization]!;
        if (auth.Length > 14)
        {
            headers[HeaderNames.Authorization] = auth[..10] + "…" + auth[^3..];
        }

        return headers;
    }
}
