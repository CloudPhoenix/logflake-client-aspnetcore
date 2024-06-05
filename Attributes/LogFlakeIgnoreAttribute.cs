namespace NLogFlake.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class LogFlakeIgnoreAttribute : Attribute
{
    public bool Ignore { get; set; }
    public LogFlakeIgnoreAttribute()
    {
        Ignore = true;
    }
}
