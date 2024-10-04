using Microsoft.AspNetCore.Http;

namespace NLogFlake.Models.Options;

public class LogFlakeMiddlewareOptions
{
    public bool GlobalExceptionHandler { get; set; } = false;

    public Func<HttpContext, Task> OnError { get; set; } = (httpContext) => Task.CompletedTask;

    public Func<HttpContext, string?> GetPerformanceMonitorLabel { get; set; } = (httpContext) => string.Empty;

    public Func<HttpContext, bool> IgnoreLogProcessing { get; set; } = (httpContext) => false;
}
