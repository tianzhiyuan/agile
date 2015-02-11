using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Framework.Caching;

namespace Agile.Framework.Redis
{
    public class RedisCache : ICacheService
    {
	    public void Set(string key, object cacheObj, TimeSpan expiredAfter)
	    {
		    throw new NotImplementedException();
	    }

	    public void Remove(string key)
	    {
		    throw new NotImplementedException();
	    }

	    public object Get(string key)
	    {
		    throw new NotImplementedException();
	    }

	    public bool Exists(string key)
	    {
		    throw new NotImplementedException();
	    }

	    public object this[string key]
	    {
		    get { throw new NotImplementedException(); }
	    }

	    public IDictionary<string, object> Get(string[] keys)
	    {
		    throw new NotImplementedException();
	    }
    }
}
