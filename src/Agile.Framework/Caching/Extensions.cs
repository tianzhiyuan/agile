using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Caching
{
    public static class Extensions
    {
        public static void Set(this ICacheService cache, string key, object obj, int minutes)
        {
	        cache.Set(key, obj, new TimeSpan(0, minutes, 0));
        }
    }
}
