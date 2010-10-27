using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using dotless.Core;
using SquishIt.Framework.Css.Compressors;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    internal class CssBundle : BundleBase, ICssBundle, ICssBundleBuilder
    {
        private List<string> _dependentFiles = new List<string>();
        private ICssCompressor _cssCompressorInstance = new MsCompressor();
        private bool _processImports = false;
        private static readonly Regex ImportPattern = new Regex(@"@import +url\(([""']){0,1}(.*?)\1{0,1}\);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public CssBundle()
            : this(new DebugStatusReader(), new FileWriterFactory(), new FileReaderFactory(), new CurrentDirectoryWrapper()) { }

        public CssBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper)
            : base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, "css", "<link rel=\"stylesheet\" type=\"text/css\" {0}href=\"{1}\" />", "~/bundle/style/") { }

        ICssBundleBuilder ICssBundle.Add(string path)
        {
            Add(path);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.Add(string path)
        {
            Add(path);
            return this;
        }

        ICssBundleBuilder ICssBundle.AddRemote(string localPath, string remotePath)
        {
            AddRemote(localPath, remotePath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.AddRemote(string localPath, string remotePath)
        {
            AddRemote(localPath, remotePath);
            return this;
        }

        ICssBundleBuilder ICssBundle.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.WithMedia(string media)
        {
            WithAttribute("media", media);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.WithAttribute(string name, string value)
        {
            WithAttribute(name, value);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.RenderOnlyIfOutputFileMissing()
        {
            RenderOnlyIfOutputFileMissing = true;
            return this;
        }

        void ICssBundleBuilder.AsNamedFile(string name, string renderTo)
        {
            AsNamed(name, renderTo, new FileRenderer(FileWriterFactory));
        }

        void ICssBundleBuilder.AsNamedCache(string name, string renderTo)
        {
            var routedPath = String.Concat(CacheRoute, renderTo);
            AsNamed(name, routedPath, new CacheRenderer(CachePrefix, renderTo));
        }

        ICssBundleBuilder ICssBundleBuilder.ForceDebug()
        {
            ForceDebug();
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.ForceRelease()
        {
            ForceRelease();
            return this;
        }

        string ICssBundle.RenderNamed(string name)
        {
            return RenderNamed(name);
        }

        string ICssBundleBuilder.RenderFile(string renderTo)
        {
            return Render(renderTo, renderTo, new FileRenderer(FileWriterFactory));
        }

        string ICssBundleBuilder.RenderCache(string renderTo)
        {
            var routedPath = String.Concat(CacheRoute, renderTo);
            return Render(renderTo, routedPath, new CacheRenderer(CachePrefix, renderTo));
        }

        public ICssBundleBuilder WithCompressor(CssCompressors cssCompressor)
        {
            _cssCompressorInstance = MapCompressorEnumToType(cssCompressor);
            return this;
        }

        public ICssBundleBuilder WithCompressor(ICssCompressor cssCompressor)
        {
            _cssCompressorInstance = cssCompressor;
            return this;
        }

        public ICssBundleBuilder ProcessImports()
        {
            _processImports = true;
            return this;
        }

        protected override string GenerateDebug()
        {
           string modifiedCssTemplate = FillTemplate("{0}");
            var processedCssFiles = new List<string>();
            foreach (string file in LocalFiles)
            {
                if (file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
                {
                    string outputFile = ResolveAppRelativePathToFileSystem(file);
                    string css = ProcessLess(outputFile);
                    outputFile += ".debug.css";
                    using (var fileWriter = FileWriterFactory.GetFileWriter(outputFile))
                    {
                        fileWriter.Write(css);
                    }
                    processedCssFiles.Add(file + ".debug.css");
                }
                else
                {
                    processedCssFiles.Add(file);
                }
            }

            return RenderFiles(modifiedCssTemplate, processedCssFiles);
        }

        protected override string GenerateRelease(string renderTo, IRenderer renderer, out IList<string> dependentFiles)
        {
            string compressedCss;
            string hash= null;
            bool hashInFileName = false;

            _dependentFiles.Clear();

            string outputFile = ResolveAppRelativePathToFileSystem(renderTo);

            List<string> files = GetFiles();
            _dependentFiles.AddRange(files);

            if (renderTo.Contains("#"))
            {
                hashInFileName = true;
                compressedCss = CompressCss(outputFile, files, _cssCompressorInstance);
                hash = Hasher.Create(compressedCss);
                renderTo = renderTo.Replace("#", hash);
                outputFile = outputFile.Replace("#", hash);
            }

            if (RenderOnlyIfOutputFileMissing && FileExists(outputFile))
            {
                compressedCss = ReadFile(outputFile);
            }
            else
            {
                compressedCss = CompressCss(outputFile, files, _cssCompressorInstance);
                renderer.Render(compressedCss, outputFile);
            }
                        
            if (hash == null)
            {
                hash = Hasher.Create(compressedCss);
            }

            string renderedCssTag;
            if (hashInFileName)
            {
                renderedCssTag = FillTemplate(ExpandAppRelativePath(renderTo));
            }
            else
            {
                string path = ExpandAppRelativePath(renderTo);
                if (path.Contains("?"))
                {
                    renderedCssTag = FillTemplate(path + "&r=" + hash);
                }
                else
                {
                    renderedCssTag = FillTemplate(path + "?r=" + hash);
                }
            }
            dependentFiles = _dependentFiles;
            return String.Concat(GetFilesForRemote(), renderedCssTag);
        }

        private ICssCompressor MapCompressorEnumToType(CssCompressors compressors)
        {
            string compressor;
            switch (compressors)
            {
                case CssCompressors.NullCompressor:
                    compressor = NullCompressor.Identifier;
                    break;
                case CssCompressors.YuiCompressor:
                    compressor = YuiCompressor.Identifier;
                    break;
                case CssCompressors.MsCompressor:
                    compressor = MsCompressor.Identifier;
                    break;
                default:
                    compressor = MsCompressor.Identifier;
                    break;
            }

            return CssCompressorRegistry.Get(compressor);
        }

        private string CompressCss(string outputFilePath, IEnumerable<string> files, ICssCompressor compressor)
        {
            var outputCss = new StringBuilder();
            foreach (string file in files)
            {
                string css;
                if (file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
                {
                    css = ProcessLess(file);
                }
                else
                {
                    css = ReadFile(file);
                }
                
                if (_processImports)
                {
                    css = ProcessImport(css);
                }
                css = CssPathRewriter.RewriteCssPaths(outputFilePath, file, css);
                outputCss.Append(compressor.CompressContent(css));
            }
            return outputCss.ToString();
        }

        private string ProcessLess(string file)
        {
            try
            {
                CurrentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                var content = ReadFile(file);
                var engineFactory = new EngineFactory();
                var engine = engineFactory.GetEngine();
                return engine.TransformToCss(content, file);
            }
            finally
            {
                CurrentDirectoryWrapper.Revert();
            }
        }

        private string ProcessImport(string css)
        {
            return ImportPattern.Replace(css, new MatchEvaluator(ApplyFileContentsToMatchedImport));
        }

        private string ApplyFileContentsToMatchedImport(Match match)
        {
            var file = ResolveAppRelativePathToFileSystem(match.Groups[2].Value);
            _dependentFiles.Add(file);
            return ReadFile(file);
        }
        
    }
}