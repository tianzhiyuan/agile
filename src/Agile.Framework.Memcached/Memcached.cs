using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Framework.Caching;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace Agile.Framework.Memcached
{
	public class Memcached : ICacheService
	{
		private readonly MemcachedClient _client;
		
		public Memcached()
		{
			_client = new MemcachedClient();
		}
		


		public void Set(string key, object cacheObj, TimeSpan expiredAfter)
		{
			_client.Store(StoreMode.Set, key, cacheObj, expiredAfter);
		}

		public void Remove(string key)
		{
			_client.Remove(key);
		}

		public object Get(string key)
		{
			return _client.Get(key);
		}

		public bool Exists(string key)
		{
			object obj;
			return _client.TryGet(key, out obj);
		}

		public object this[string key]
		{
			get { return _client.Get(key); }
		}

		public IDictionary<string, object> Get(string[] keys)
		{
			return _client.Get(keys);
		}
	}
}
