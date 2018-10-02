namespace Common.Interface
{
    public interface IETagCache
    {
        T GetCachedObject<T>(string cacheKeyName);
        bool SetCachedObject(string cacheKeyName, dynamic objectToCache);
    }
}
