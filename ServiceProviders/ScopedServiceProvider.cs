using System;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// A service provider that releases all of its instances when its disposed, for DI that should only apply within a scope (ex web request)
    /// </summary>
    public class ScopedServiceProvider : AbstractServiceProvider
    {
        /// <summary>
        /// Adds a new object instance to the provided type registrations list of instances
        /// </summary>
        /// <param name="t">The type registration to hold the instance</param>
        /// <param name="o">The object instance to add</param>
        public override void Add(Type t, object o)
        {
            if (Instances.TryGetValue(t, out List<object> instances))
            {
                instances.Add(o);
            }
            else
            {
                Instances.Add(t, new List<object>() { o });
            }
        }

        /// <summary>
        /// Gets a LIST of object instances by the registered type
        /// </summary>
        /// <param name="t">The registration type for the object to retrieve</param>
        /// <returns>A LIST of object instances that are part of the type registration</returns>
        public override object GetService(Type t)
        {
            return Instances.TryGetValue(t, out List<object> instances) ? instances : new List<object>();
        }

        /// <summary>
        /// A list of all the objects that were constructed in the scope containing this service provider (set to be scoped)
        /// </summary>
        protected Dictionary<Type, List<object>> Instances { get; set; } = new Dictionary<Type, List<object>>();
    }
}