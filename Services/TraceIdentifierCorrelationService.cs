using Microsoft.AspNetCore.Http;

namespace NLogFlake.Services;

public class TraceIdentifierCorrelationService : ICorrelationService
{
    private readonly string _correlationId;

    public string Correlation => _correlationId.ToString();

    public TraceIdentifierCorrelationService(IHttpContextAccessor httpContextAccessor)
    {
        _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
    }
}
