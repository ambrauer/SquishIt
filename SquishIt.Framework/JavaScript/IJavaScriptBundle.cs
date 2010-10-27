namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundle : IBundle
    {
        IJavaScriptBundleBuilder Add(string path);
        IJavaScriptBundleBuilder AddRemote(string localPath, string remotePath);
        IJavaScriptBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        string RenderNamed(string name);
    }
}