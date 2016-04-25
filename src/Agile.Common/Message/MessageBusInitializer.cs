using System;
using System.Linq;
using System.Reflection;
using Agile.Common.Components;

namespace Agile.Common.Message
{
    [Component]
    public class MessageBusInitializer : IAssemblyInitializer
    {

        private static bool IsBaseListener(Type type)
        {
            return type.IsClass && !type.IsAbstract && 
                type.BaseType != null && type.BaseType.IsGenericType &&
                   type.BaseType.GetGenericTypeDefinition() == typeof (BaseListener<>);
        }

        public void Initialize(Assembly[] assemblies)
        {
            var bus = ObjectContainer.Resolve<IMessageBus>();
            var listeners = ObjectContainer.ResolveAll<IListener>().Where(o => IsBaseListener(o.GetType()));
            foreach (var listener in listeners)
            {
                var messageType = listener.GetType().BaseType.GetGenericArguments()[0];
                bus.Subscribe(listener, messageType);
            }
        }
    }
}
