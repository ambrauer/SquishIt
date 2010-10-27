namespace SquishIt.Framework.Css
{
    public interface ICssBundle : IBundle
    {
        ICssBundleBuilder Add(string path);
        ICssBundleBuilder AddRemote(string localPath, string remotePath);
        ICssBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        string RenderNamed(string name);
    }
}