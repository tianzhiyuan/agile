using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using MC = System.Runtime.Caching.MemoryCache;
namespace Agile.Framework.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly ObjectCache CacheService = MC.Default;
        
        public void Set(string Key, object cacheObj, TimeSpan expiredAfter)
        {
            CacheService.Set(Key, cacheObj, DateTime.Now.Add(expiredAfter));
        }
        public void Remove(string Key)
        {
            CacheService.Remove(Key);
        }

        public object Get(string Key)
        {
            return CacheService.Get(Key);
        }

	    public bool Exists(string key)
	    {
		    return CacheService.Contains(key);
	    }

	    public object this[string key]
	    {
			get { return Get(key); }
	    }

	    public IDictionary<string, object> Get(string[] keys)
	    {
		    return CacheService.GetValues(keys);
	    }

	    
        
    }
}
