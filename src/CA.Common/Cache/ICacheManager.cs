namespace CA.Common.Cache
{
    public interface ICacheManager
    {
        void Add(string key, object value);
        T Get<T>(string key) where T : class;
        void Clear(string key);
    }
}
