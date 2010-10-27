using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SquishIt.Framework.Renderers
{
    public class CacheRenderer: IRenderer
    {
        private string _prefix;
        private string _key;
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        public CacheRenderer(string prefix, string key)
        {
            _prefix = prefix;
            _key = key;
        }

        public void Render(string content, string outputFile)
        {
            if (_key.Contains("#"))
            {
                // Need to adjust key to include the generated hash
                // Example:
                //      outputFile = "/bundle/style/output_234234234234234.js"
                //      _key = "output_#.js"
                var parts = _key.Split('#');
                var hash = parts.Aggregate(outputFile, (current, part) => current.Replace(part, string.Empty));
                var i = hash.LastIndexOfAny(new []{'/','\\'});
                if (i > 0)
                {
                    hash = hash.Remove(0, i+1);
                }
                _key = _key.Replace("#", hash);
            }
            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                _cache[_prefix + "_" + _key] = content;
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }

        public string Get()
        {
            _readerWriterLockSlim.EnterReadLock();
            try
            {
                return _cache[_prefix + "_" + _key];
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }
    }
}