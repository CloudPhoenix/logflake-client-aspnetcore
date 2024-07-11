namespace NLogFlake.Services;

public interface IParameterService
{
    Dictionary<string, object> Get();

    void AddOrUpdate(string key, object value);

    bool IsEmpty();
}
