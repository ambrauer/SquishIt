using SquishIt.Framework.JavaScript.Minifiers;

namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundleBuilder
    {
        IJavaScriptBundleBuilder Add(string path);
        IJavaScriptBundleBuilder AddRemote(string localPath, string remotePath);
        IJavaScriptBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier);
        IJavaScriptBundleBuilder WithMinifier(IJavaScriptMinifier javaScriptMinifier);
        IJavaScriptBundleBuilder RenderOnlyIfOutputFileMissing();
        string RenderFile(string renderTo);
        string RenderCache(string renderTo);
        void AsNamedFile(string name, string renderTo);
        void AsNamedCache(string name, string renderTo);
        IJavaScriptBundleBuilder ForceDebug();
        IJavaScriptBundleBuilder ForceRelease();
        IJavaScriptBundleBuilder WithAttribute(string name, string value);
    }
}