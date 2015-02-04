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
	public class Memcached : ICache
	{
		private readonly MemcachedClient _client;
		
		public Memcached()
		{
			_client = new MemcachedClient();
		}
		public void AddOrUpdate(string key, object cacheObj, TimeSpan expiredAfter)
		{
			_client.Store(StoreMode.Set, key, cacheObj, expiredAfter);
			
		}

		public void Delete(string key)
		{
			_client.Remove(key);
		}

		public object Get(string key)
		{
			return _client.Get(key);
		}

		
		#region ICache.Members
		void ICache.AddOrUpdate(string Key, object cacheObj, TimeSpan expiredAfter)
		{
			this.AddOrUpdate(Key, cacheObj, expiredAfter);
		}

		void ICache.Delete(string Key)
		{
			this.Delete(Key);
		}

		object ICache.Get(string Key)
		{
			return this.Get(Key);
		}

		IEnumerable<string> ICache.Keys
		{
			get { throw new NotSupportedException(); }
		}

		void ICache.Clear()
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}
