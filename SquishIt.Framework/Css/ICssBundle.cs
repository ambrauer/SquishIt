namespace SquishIt.Framework.Css
{
    /// <summary>
    /// Interface for css bundle
    /// </summary>
    public interface ICssBundle : IBundle
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
        /// Outputs the appropriate 'link' tag of a previously named bundle - created with either the 'AsNamedFile' or 'AsNamedCache' methods.
        /// </summary>
        /// <param name="name">The name of the bundle.</param>
        string RenderNamed(string name);
    }
}