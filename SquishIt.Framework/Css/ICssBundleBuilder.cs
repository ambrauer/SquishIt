using SquishIt.Framework.Css.Compressors;

namespace SquishIt.Framework.Css
{
    public interface ICssBundleBuilder
    {
        ICssBundleBuilder Add(string path);
        ICssBundleBuilder AddRemote(string localPath, string remotePath);
        ICssBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        ICssBundleBuilder WithMedia(string media);
        ICssBundleBuilder WithCompressor(CssCompressors cssCompressor);
        ICssBundleBuilder WithCompressor(ICssCompressor cssCompressor);
        string RenderFile(string renderTo);
        string RenderCache(string renderTo);
        void AsNamedFile(string name, string renderTo);
        void AsNamedCache(string name, string renderTo);
        ICssBundleBuilder RenderOnlyIfOutputFileMissing();
        ICssBundleBuilder ForceDebug();
        ICssBundleBuilder ForceRelease();
        ICssBundleBuilder ProcessImports();
        ICssBundleBuilder WithAttribute(string name, string value);
    }
}