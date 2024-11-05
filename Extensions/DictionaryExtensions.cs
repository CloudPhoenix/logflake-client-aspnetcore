namespace NLogFlake.Extensions;

internal static class DictionaryExtensions
{
    internal static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> newParameters)
    {
        Dictionary<TKey, TValue> result = new(source);

        foreach (KeyValuePair<TKey, TValue> item in newParameters)
        {
            result[item.Key] = item.Value;
        }

        return result;
    }
}
