using Microsoft.AspNetCore.Builder;
using NLogFlake.Middlewares;

namespace NLogFlake;

public static class IApplicationBuilderExtensions
{
    public static void UseLogFlakeMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<LogFlakeMiddleware>();
}
