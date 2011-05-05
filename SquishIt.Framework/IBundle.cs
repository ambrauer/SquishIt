namespace SquishIt.Framework
{
    /// <summary>
    /// Base interface for all bundles
    /// </summary>
    public interface IBundle
    {
        /// <summary>
        /// Clears the testing cache.
        /// </summary>
        void ClearTestingCache();

        /// <summary>
        /// Sets the path that will be used when the bundle is requested. For Javascript, the default is "~/bundle/script/"; for CSS, "~/bundle/style/".
        /// </summary>
        /// <param name="route">The route.</param>
        void SetCacheRoute(string route);

        /// <summary>
        /// Gets the cached content for a given key (bundle file name).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetCachedContent(string key);
    }
}
