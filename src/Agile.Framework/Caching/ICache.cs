using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Caching
{
    /// <summary>
    /// Caching
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Add or update object with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheObj"></param>
        /// <param name="expiredAfter"></param>
        void AddOrUpdate(string key, object cacheObj, TimeSpan expiredAfter);
        /// <summary>
        /// delete cached object by key
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);
        /// <summary>
        /// Get cached object by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// Get list of Keys
        /// </summary>
        IEnumerable<string> Keys { get; }
        /// <summary>
        /// Clear all cachedd objects
        /// </summary>
        void Clear();
    }
}
