using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Utils
{
    public static class CollectionUtils
    {
        public static bool NotEmpty<T>(IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }
        public static bool IsEmpty<T>(IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> func)
        {
            foreach (var c in collection)
            {
                var local = c;
                func.Invoke(local);
            }
        }
    }
}
