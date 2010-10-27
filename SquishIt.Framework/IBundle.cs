namespace SquishIt.Framework
{
    public interface IBundle
    {
        void ClearTestingCache();
        void SetCacheRoute(string route);
        string GetCachedContent(string key);
    }
}
