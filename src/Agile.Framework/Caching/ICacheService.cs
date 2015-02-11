using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Caching
{
    /// <summary>
    /// Caching interface
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// set object with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheObj"></param>
        /// <param name="expiredAfter"></param>
        void Set(string key, object cacheObj, TimeSpan expiredAfter);
        /// <summary>
        /// remove cached object by key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
        /// <summary>
        /// Get cached object by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
	    /// <summary>
	    /// check if exist
	    /// </summary>
	    bool Exists(string key);
		/// <summary>
		/// get/set a cached object using an index
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object this[string key] { get; }
		/// <summary>
		/// get a set of cached objects
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
	    IDictionary<string, object> Get(string[] keys);
    }
}
