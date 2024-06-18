namespace NLogFlake.Models.Options;

public sealed class LogFlakeMiddlewareSettingsOptions
{
    public const string SectionName = "LogFlakeMiddlewareSettings";

    public bool LogRequest { get; set; }

    public bool LogResponse { get; set; }

    public bool LogNotFoundErrors { get; set; }

    public string? ClientIdSelector { get; set; }
}
