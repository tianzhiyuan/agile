using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Components;
using Agile.Common.Data;

namespace Agile.Common.Utilities
{
    public static class TypeUtils
    {
        public static bool Is(Type me, Type baseType)
        {
            return
                (me.IsGenericType && me.GetGenericTypeDefinition() == baseType)
                || (me.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == baseType))
                || baseType.IsAssignableFrom(me);
        }

        public static bool IsComponent(Type type)
        {
            return type != null && type.IsClass && type.GetCustomAttributes(typeof(ComponentAttribute), false).Any();
        }

        public static bool IsAssemblyInitializer(Type type)
        {
            return type != null && type.IsClass && !type.IsAbstract &&
                   typeof (IAssemblyInitializer).IsAssignableFrom(type);
        }

        public static bool IsBaseEntity(Type type)
        {
            return type != null && type.IsClass && !type.IsAbstract && typeof (BaseEntity).IsAssignableFrom(type);
        }

		public static bool IsBaseEntityQuery(Type type)
		{
			return type != null && type.IsClass && !type.IsAbstract && !type.IsGenericType &&
			       typeof (BaseEntityQuery).IsAssignableFrom(type);
		}
    }
}
