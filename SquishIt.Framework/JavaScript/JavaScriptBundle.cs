using System;
using System.Collections.Generic;
using System.Text;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    internal class JavaScriptBundle: BundleBase, IJavaScriptBundle, IJavaScriptBundleBuilder
    {
        private IJavaScriptMinifier _javaScriptMinifier = new MsMinifier();
        
        public JavaScriptBundle()
            : this(new DebugStatusReader(), new FileWriterFactory(), new FileReaderFactory(), new CurrentDirectoryWrapper()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper):
            base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, "js", "<script type=\"text/javascript\" {0}src=\"{1}\"></script>", "~/bundle/script/") { }

        IJavaScriptBundleBuilder IJavaScriptBundle.Add(string path)
        {
            Add(path);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.Add(string path)
        {
            Add(path);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundle.AddRemote(string localPath, string remotePath)
        {
            AddRemote(localPath, remotePath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.AddRemote(string localPath, string remotePath)
        {
            AddRemote(localPath, remotePath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundle.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.WithAttribute(string name, string value)
        {
            WithAttribute(name, value);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.RenderOnlyIfOutputFileMissing()
        {
            RenderOnlyIfOutputFileMissing = true;
            return this;
        }

        void IJavaScriptBundleBuilder.AsNamedFile(string name, string renderTo)
        {
            AsNamed(name, renderTo, new FileRenderer(FileWriterFactory));
        }

        void IJavaScriptBundleBuilder.AsNamedCache(string name, string renderTo)
        {
            var routedPath = String.Concat(CacheRoute, renderTo);
            AsNamed(name, routedPath, new CacheRenderer(CachePrefix, renderTo));
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.ForceDebug()
        {
            ForceDebug();
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.ForceRelease()
        {
            ForceRelease();
            return this;
        }

        string IJavaScriptBundle.RenderNamed(string name)
        {
            return RenderNamed(name);
        }

        string IJavaScriptBundleBuilder.RenderFile(string renderTo)
        {
            return Render(renderTo, renderTo, new FileRenderer(FileWriterFactory));
        }

        string IJavaScriptBundleBuilder.RenderCache(string renderTo)
        {
            var routedPath = String.Concat(CacheRoute, renderTo);
            return Render(renderTo, routedPath, new CacheRenderer(CachePrefix, renderTo));
        }

        public IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier)
        {
            this._javaScriptMinifier = MapMinifierEnumToType(javaScriptMinifier);
            return this;
        }

        public IJavaScriptBundleBuilder WithMinifier(IJavaScriptMinifier javaScriptMinifier)
        {
            this._javaScriptMinifier = javaScriptMinifier;
            return this;
        }

        protected override string GenerateDebug()
        {
            string modifiedTemplate = FillTemplate("{0}");
            return RenderFiles(modifiedTemplate, LocalFiles);
        }

        protected override string GenerateRelease(string renderTo, IRenderer renderer, out IList<string> dependentFiles)
        {
            string compressedJavaScript;
            string hash = null;
            bool hashInFileName = false;

            List<string> files = GetFiles();
                        
            if (renderTo.Contains("#"))
            {
                hashInFileName = true;
                compressedJavaScript = MinifyJavaScript(files, _javaScriptMinifier);
                hash = Hasher.Create(compressedJavaScript);
                renderTo = renderTo.Replace("#", hash);
            }

            var outputFile = ResolveAppRelativePathToFileSystem(renderTo);

            string minifiedJavaScript;
            if (RenderOnlyIfOutputFileMissing && FileExists(outputFile))
            {
                minifiedJavaScript = ReadFile(outputFile);
            }
            else
            {
                minifiedJavaScript = MinifyJavaScript(files, _javaScriptMinifier);
                renderer.Render(minifiedJavaScript, outputFile);
            }
                        
            if (hash == null)
            {
                hash = Hasher.Create(minifiedJavaScript);                            
            }
                        
            string renderedScriptTag;
            if (hashInFileName)
            {
                renderedScriptTag = FillTemplate(ExpandAppRelativePath(renderTo));
            }
            else
            {
                string path = ExpandAppRelativePath(renderTo);
                if (path.Contains("?"))
                {
                    renderedScriptTag = FillTemplate(ExpandAppRelativePath(renderTo) + "&r=" + hash);    
                }
                else
                {
                    renderedScriptTag = FillTemplate(ExpandAppRelativePath(renderTo) + "?r=" + hash);        
                }
            }
            dependentFiles = files;
            return String.Concat(GetFilesForRemote(), renderedScriptTag);
        }

        private IJavaScriptMinifier MapMinifierEnumToType(JavaScriptMinifiers javaScriptMinifier)
        {
            string minifier;
            switch (javaScriptMinifier)
            {
                case JavaScriptMinifiers.NullMinifier:
                    minifier = NullMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.JsMin:
                    minifier = JsMinMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.Closure:
                    minifier = ClosureMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.Yui:
                    minifier = YuiMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.Ms:
                    minifier = MsMinifier.Identifier;
                    break;
                default:
                    minifier = MsMinifier.Identifier;
                    break;
            }
            return MinifierRegistry.Get(minifier);
        }

        private string MinifyJavaScript(IEnumerable<string> files, IJavaScriptMinifier minifier)
        {
            try
            {
                var inputJavaScript = new StringBuilder();
                foreach (var file in files)
                {
                    inputJavaScript.Append(ReadFile(file));
                }
                return minifier.CompressContent(inputJavaScript.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error processing: {0}", e.Message), e);
            }
        }

    }
}