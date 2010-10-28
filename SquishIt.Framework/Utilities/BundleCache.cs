using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SquishIt.Framework.Utilities
{
    public class BundleCache
    {
        private string _prefix;
        private static Dictionary<string, string> cache = new Dictionary<string, string>();

        public BundleCache(string prefix)
        {
            _prefix = prefix;
        }

        public string GetContent(string name)
        {
            if (HttpContext.Current != null)
            {
                return (string)HttpContext.Current.Cache["squishit_" + _prefix + "_" + name];
            }
            return cache[_prefix + "_" + name];
        }

        public void ClearTestingCache()
        {
            cache.Clear();
        }

        public bool ContainsKey(string key)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Cache["squishit_" + _prefix + "_" + key] != null;
            }
            return cache.ContainsKey(_prefix + "_" + key);
        }

        public void AddToCache(string key, string content, IList<string> files)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Add("squishit_" + _prefix + "_" + key, content, new CacheDependency(files.ToArray()),
                                                Cache.NoAbsoluteExpiration, 
                                                new TimeSpan(365, 0, 0, 0),
                                                CacheItemPriority.NotRemovable,
                                                null);
            }
            else
            {
                cache.Add(_prefix + "_" + key, content);    
            }
        }
    }
}