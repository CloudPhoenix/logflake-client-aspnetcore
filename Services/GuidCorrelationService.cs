using Microsoft.AspNetCore.Http;

namespace NLogFlake.Services;

public class GuidCorrelationService : ICorrelationService
{
    private readonly Guid _correlation;

    public string Correlation => _correlation.ToString();

    public GuidCorrelationService()
    {
        _correlation = Guid.NewGuid();
    }
}
