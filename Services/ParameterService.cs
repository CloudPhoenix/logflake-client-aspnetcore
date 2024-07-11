
namespace NLogFlake.Services;

public class ParameterService : IParameterService
{
    private readonly Dictionary<string, object> _parameters;

    public ParameterService()
    {
        _parameters = [];
    }

    public void AddOrUpdate(string key, object value) => _parameters[key] = value;

    public Dictionary<string, object> Get() => _parameters;

    public bool IsEmpty() => _parameters.Count == 0;
}
