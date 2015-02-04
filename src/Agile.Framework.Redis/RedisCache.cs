using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Framework.Caching;

namespace Agile.Framework.Redis
{
    public class RedisCache : ICache
    {
	    public void AddOrUpdate(string key, object cacheObj, TimeSpan expiredAfter)
	    {
		    throw new NotImplementedException();
	    }

	    public void Delete(string key)
	    {
		    throw new NotImplementedException();
	    }

	    public object Get(string key)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<string> Keys { get; private set; }
	    public void Clear()
	    {
		    throw new NotImplementedException();
	    }
    }
}
