using SquishIt.Framework.Css.Compressors;

namespace SquishIt.Framework.Css
{
    /// <summary>
    /// Interface for css bundle builder
    /// </summary>
    public interface ICssBundleBuilder
    {
        /// <summary>
        /// Add a file to the bundle. e.g. "~/Content/Styles/somefile.css"
        /// </summary>
        /// <param name="path">The file path.</param>
        ICssBundleBuilder Add(string path);
        /// <summary>
        /// Add a remote file to the bundle.
        /// </summary>
        /// <param name="localPath">The local file path.</param>
        /// <param name="remotePath">The remote file path.</param>
        ICssBundleBuilder AddRemote(string localPath, string remotePath);
        /// <summary>
        /// Add an embedded resource to the bundle.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <param name="embeddedResourcePath">The embedded resource path.</param>
        ICssBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        /// <summary>
        /// Specify the media type. This will be set to the 'media' attribute of the generated 'link' tag.
        /// </summary>
        /// <param name="media">The media type.</param>
        ICssBundleBuilder WithMedia(string media);
        /// <summary>
        /// Specify the compressor to use. Default (and recommended) is AjaxMin.
        /// </summary>
        /// <param name="cssCompressor">The CSS compressor.</param>
        ICssBundleBuilder WithCompressor(CssCompressors cssCompressor);
        /// <summary>
        /// Specify the compressor to use. Default (and recommended) is AjaxMin.
        /// </summary>
        /// <param name="cssCompressor">The CSS compressor.</param>
        ICssBundleBuilder WithCompressor(ICssCompressor cssCompressor);
        /// <summary>
        /// Outputs the appropriate 'link' tag and writes the specified bundle file to disk. e.g. "~/Content/Styles/combined_#.css".
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our css files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="renderTo">The output bundle file path. e.g. "~/Content/Styles/combined_#.css"</param>
        string RenderFile(string renderTo);
        /// <summary>
        /// Outputs the appropriate 'link' tag and writes the specified bundle file to cache. e.g. "combined_#.css".
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our css files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="renderTo">The output bundle file name (path is not necessary since we are only writing to cache, not to disk). e.g. "combined_#.css"</param>
        string RenderCache(string renderTo);
        /// <summary>
        /// Writes the specified bundle file to disk (e.g. "~/Content/Styles/combined_#.css") which can be accessed later by the given key name via 'RenderNamed'.
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our css files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="renderTo">The output bundle file path. e.g. "~/Content/Styles/combined_#.css"</param>
        void AsNamedFile(string name, string renderTo);
        /// <summary>
        /// Writes the specified bundle file to cache (e.g. "combined_#.css") which can be accessed later by the given key name via 'RenderNamed'.
        /// 
        /// Notice the "_#" in the output file name. This will cause bundler to render a hash of the file contents into the filename. 
        /// This allows us to invalidate the file if a change is made to one of our css files. If you don't include that in there, 
        /// then the hash is appended as a querystring parameter. This will also work for invalidating the file, but could cause some 
        /// caching proxies not to cache the file.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="renderTo">The output bundle file name (path is not necessary since we are only writing to cache, not to disk). e.g. "combined_#.css"</param>
        void AsNamedCache(string name, string renderTo);
        /// <summary>
        /// Renders the file only if the specified output file missing. This is really only useful in conjunction with RenderFile/AsNamedFile.
        /// </summary>
        ICssBundleBuilder RenderOnlyIfOutputFileMissing();
        /// <summary>
        /// Force to render in debug mode (i.e. spit out individual files - no bundling/compressing).
        /// This can also be done at runtime by passing in a query string parameter of DebugMode set to True (e.g. ?DebugMode=True)
        /// </summary>
        ICssBundleBuilder ForceDebug();
        /// <summary>
        /// Force to render in release mode (i.e. spit out compressed bundle - no individual files).
        /// This can also be done at runtime by passing in a query string parameter of DebugMode set to False (e.g. ?DebugMode=False)
        /// </summary>
        ICssBundleBuilder ForceRelease();
        /// <summary>
        /// Process CSS imports.
        /// </summary>
        ICssBundleBuilder ProcessImports();
        /// <summary>
        /// Specify an additional attribute to be added to the rendered 'link' tag.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        ICssBundleBuilder WithAttribute(string name, string value);
    }
}