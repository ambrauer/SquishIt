using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SquishIt.Framework.FileResolvers;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework
{
    public abstract class BundleBase
    {
        protected IFileWriterFactory FileWriterFactory { get; private set; }
        protected IFileReaderFactory FileReaderFactory { get; private set; }
        protected IDebugStatusReader DebugStatusReader { get; private set; }
        protected ICurrentDirectoryWrapper CurrentDirectoryWrapper { get; private set; }

        private static BundleCache _bundleCache = new BundleCache();
        private static Dictionary<string, string> _debugFiles = new Dictionary<string, string>();
        private static Dictionary<string, NamedState> _namedState = new Dictionary<string, NamedState>();

        private Dictionary<string, string> _attributes = new Dictionary<string, string>();
        private IList<string> _localFiles = new List<string>();
        private IList<string> _remoteFiles = new List<string>();
        private IList<string> _embeddedResourceFiles = new List<string>();

        protected string CachePrefix { get; private set; }
        protected string TagTemplate { get; private set; }

        protected IList<string> LocalFiles { get { return _localFiles; } }
        protected bool RenderOnlyIfOutputFileMissing { get; set; }
        protected string CacheRoute { get; set; }

        protected BundleBase(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDebugStatusReader debugStatusReader, ICurrentDirectoryWrapper currentDirectoryWrapper, string cachePrefix, string tagTemplate, string cacheRoute)
        {
            FileWriterFactory = fileWriterFactory;
            FileReaderFactory = fileReaderFactory;
            DebugStatusReader = debugStatusReader;
            CurrentDirectoryWrapper = currentDirectoryWrapper;

            CachePrefix = cachePrefix;
            TagTemplate = tagTemplate;
            CacheRoute = cacheRoute;
            RenderOnlyIfOutputFileMissing = false;
        }

        #region File Helpers (protected)

        protected string RenderFiles(string template, IEnumerable<string> files)
        {
            var sb = new StringBuilder();
            foreach (string file in files)
            {
                string processedFile = ExpandAppRelativePath(file);
                sb.Append(String.Format(template, processedFile));
            }
            return sb.ToString();
        }

        protected List<string> GetFiles()
        {
            var files = new List<string>();
            var fileResolverCollection = new FileResolverCollection();

            foreach (var file in _localFiles)
            {
                string mappedPath = ResolveAppRelativePathToFileSystem(file);
                files.AddRange(fileResolverCollection.Resolve(mappedPath, FileResolver.Type));
            }
            foreach (var file in _embeddedResourceFiles)
            {
                files.AddRange(fileResolverCollection.Resolve(file, EmbeddedResourceResolver.Type));
            }

            return files;
        }

        //protected void WriteGZippedFile(string outputJavaScript, string gzippedOutputFile)
        //{
        //    if (gzippedOutputFile != null)
        //    {
        //        var gzipper = new FileGZipper();
        //        gzipper.Zip(gzippedOutputFile, outputJavaScript);
        //    }
        //}

        protected string GetFilesForRemote()
        {
            var renderedFilesForCdn = new StringBuilder();
            foreach (var uri in _remoteFiles)
            {
                renderedFilesForCdn.Append(FillTemplate(uri));
            }
            return renderedFilesForCdn.ToString();
        }

        protected string FillTemplate(string path)
        {
            return String.Format(TagTemplate, GetAdditionalAttributes(), path);
        }

        protected string ResolveAppRelativePathToFileSystem(string file)
        {
            // Remove query string
            if (file.IndexOf('?') != -1)
            {
                file = file.Substring(0, file.IndexOf('?'));
            }
            
            if (HttpContext.Current == null)
            {
                file = file.Replace("/", "\\").TrimStart('~').TrimStart('\\');
                return @"C:\" + file.Replace("/", "\\");
            }
            return HttpContext.Current.Server.MapPath(file);
        }

        protected string ExpandAppRelativePath(string file)
        {            
            if (file.StartsWith("~/"))
            {
                string appRelativePath = HttpRuntime.AppDomainAppVirtualPath;
                if (appRelativePath != null && !appRelativePath.EndsWith("/"))
                    appRelativePath += "/";
                return file.Replace("~/", appRelativePath);    
            }
            return file;
        }

        protected string ReadFile(string file)
        {
            using (var sr = FileReaderFactory.GetFileReader(file))
            {
                return sr.ReadToEnd();
            }
        }

        protected bool FileExists(string file)
        {
            return FileReaderFactory.FileExists(file);
        }

        #endregion

        #region API Methods (protected)

        protected void Add(string path)
        {
            _localFiles.Add(path);
        }

        protected void AddRemote(string localPath, string remotePath)
        {
            if (DebugStatusReader.IsDebuggingEnabled())
            {
                _localFiles.Add(localPath);
            }
            else
            {
                _remoteFiles.Add(remotePath);
            }
        }

        protected void AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            if (DebugStatusReader.IsDebuggingEnabled())
            {
                _localFiles.Add(localPath);
            }
            else
            {
                _embeddedResourceFiles.Add(embeddedResourcePath);
            }
        }

        protected void WithAttribute(string name, string value)
        {
            if (_attributes.ContainsKey(name))
            {
                _attributes[name] = value;
            }
            else
            {
                _attributes.Add(name, value);
            }
        }

        protected void AsNamed(string name, string renderTo, IRenderer renderer)
        {
            _namedState[name] = new NamedState(DebugStatusReader.IsForced() ? DebugStatusReader.IsDebuggingEnabled() : (bool?)null, renderTo);
            // Need to prime both debug and release so both are available for real-time debugging (?debugMode=true)
            RenderDebug(name);
            RenderRelease(name, renderTo, renderer);
        }

        protected void ForceDebug()
        {
            DebugStatusReader.ForceDebug();
        }

        protected void ForceRelease()
        {
            DebugStatusReader.ForceRelease();
        }

        protected string RenderNamed(string name)
        {
            NamedState state = _namedState[name];
            if (state.Debug.HasValue)
            {
                // If forced when creating the bundle, then use that setting
                return state.Debug.Value ? _debugFiles[name] : _bundleCache.GetContent(name);
            }
            return DebugStatusReader.IsDebuggingEnabled() ? _debugFiles[name] : _bundleCache.GetContent(name);
        }

        protected string Render(string key, string renderTo, IRenderer renderer)
        {
            return DebugStatusReader.IsDebuggingEnabled() ? RenderDebug(key) : RenderRelease(key, renderTo, renderer);
        }

        #endregion

        #region Generate Methods (protected virtual)

        protected virtual string GenerateDebug()
        {
            return String.Empty;
        }

        protected virtual string GenerateRelease(string renderTo, IRenderer renderer, out IList<string> dependentFiles)
        {
            dependentFiles = new List<string>();
            return String.Empty;
        }

        #endregion

        #region IBundle Methods (public)

        public void ClearTestingCache()
        {
            _bundleCache.ClearTestingCache();
            _debugFiles.Clear();
            _namedState.Clear();
        }

        public void SetCacheRoute(string route)
        {
            CacheRoute = route;
        }

        public string GetCachedContent(string key)
        {
            var cacheRenderer = new CacheRenderer(CachePrefix, key);
            return cacheRenderer.Get();
        }

        #endregion

        private string GetAdditionalAttributes()
        {
            var result = new StringBuilder();
            foreach (string key in _attributes.Keys)
            {
                result.Append(key);
                result.Append("=\"");
                result.Append(_attributes[key]);
                result.Append("\" ");
            }
            return result.ToString();
        }

        private string RenderDebug(string key)
        {
            if (!_debugFiles.ContainsKey(key))
            {
                _debugFiles[key] = GenerateDebug();
            }
            return _debugFiles[key];
        }

        private string RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            if (!_bundleCache.ContainsKey(key))
            {
                lock (_bundleCache)
                {
                    if (!_bundleCache.ContainsKey(key))
                    {
                        IList<string> dependentFiles;
                        var content = GenerateRelease(renderTo, renderer, out dependentFiles);
                        _bundleCache.AddToCache(key, content, dependentFiles);
                    }
                }
            }
            return _bundleCache.GetContent(key);
        }
    }
}