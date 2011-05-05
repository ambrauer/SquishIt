using SquishIt.Framework.JavaScript.Minifiers;

namespace SquishIt.Framework.JavaScript
{
    /// <summary>
    /// Interface for javascript bundle builder
    /// </summary>
    public interface IJavaScriptBundleBuilder
    {
        /// <summary>
        /// Add a file to the bundle. e.g. "~/Content/Scripts/somefile.js"
        /// </summary>
        /// <param name="path">The file path.</param>
        IJavaScriptBundleBuilder Add(string path);
        /// <summary>
        /// Add a remote file to the bundle.
        /// </summary>
        /// <param name="localPath">The local file path.</param>
        /// <param name="remotePath">The remote file path.</param>
        IJavaScriptBundleBuilder AddRemote(string localPath, string remotePath);
        /// <summary>
        /// Add an embedded resource to the bundle.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <param name="embeddedResourcePath">The embedded resource path.</param>
        IJavaScriptBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        /// <summary>
        /// Specify the minifier to use. Default (and recommended) is AjaxMin.
        /// </summary>
        /// <param name="javaScriptMinifier">The javascript minifier.</param>
        IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier);
        /// <summary>
        /// Specify the minifier to use. Default (and recommended) is AjaxMin.
        /// </summary>
        /// <param name="javaScriptMinifier">The javascript minifier.</param>
        IJavaScriptBundleBuilder WithMinifier(IJavaScriptMinifier javaScriptMinifier);
        /// <summary>
        /// Renders the file only if the specified output file missing. This is really only useful in conjunction with RenderFile/AsNamedFile.
        /// </summary>
        IJavaScriptBundleBuilder RenderOnlyIfOutputFileMissing();
        /// <summary>
        /// Outputs the appropriate 'script' tag and writes the specified bundle file to disk. e.g. "~/Content/Scripts/combined_#.js".
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our js files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="renderTo">The output bundle file path. e.g. "~/Content/Scripts/combined_#.js"</param>
        string RenderFile(string renderTo);
        /// <summary>
        /// Outputs the appropriate 'script' tag and writes the specified bundle file to cache. e.g. "combined_#.js".
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our js files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="renderTo">The output bundle file name (path is not necessary since we are only writing to cache, not to disk). e.g. "combined_#.js"</param>
        string RenderCache(string renderTo);
        /// <summary>
        /// Writes the specified bundle file to disk (e.g. "~/Content/Scripts/combined_#.js") which can be accessed later by the given key name via 'RenderNamed'.
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our js files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="renderTo">The output bundle file path. e.g. "~/Content/Scripts/combined_#.js"</param>
        void AsNamedFile(string name, string renderTo);
        /// <summary>
        /// Writes the specified bundle file to cache (e.g. "combined_#.js") which can be accessed later by the given key name via 'RenderNamed'.
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our js files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="renderTo">The output bundle file name (path is not necessary since we are only writing to cache, not to disk). e.g. "combined_#.js"</param>
        void AsNamedCache(string name, string renderTo);
        /// <summary>
        /// Force to render in debug mode (i.e. spit out individual files - no bundling/minifying).
        /// This can also be done at runtime by passing in a query string parameter of DebugMode set to True (e.g. ?DebugMode=True)
        /// </summary>
        IJavaScriptBundleBuilder ForceDebug();
        /// <summary>
        /// Force to render in release mode (i.e. spit out minified bundle - no individual files)
        /// This can also be done at runtime by passing in a query string parameter of DebugMode set to False (e.g. ?DebugMode=False)
        /// </summary>
        IJavaScriptBundleBuilder ForceRelease();
        /// <summary>
        /// Specify an additional attribute to be added to the rendered 'script' tag.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        IJavaScriptBundleBuilder WithAttribute(string name, string value);
    }
}