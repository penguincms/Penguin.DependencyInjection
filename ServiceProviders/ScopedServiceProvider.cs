﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// A service provider that releases all of its instances when its disposed, for DI that should only apply within a scope (ex web request)
    /// </summary>
    public class ScopedServiceProvider : AbstractServiceProvider
    {
        /// <summary>
        /// A list of all the objects that were constructed in the scope containing this service provider (set to be scoped)
        /// </summary>
        protected Dictionary<Type, List<object>> Instances { get; } = new Dictionary<Type, List<object>>();

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
        /// Clears out the scoped services
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<Type, List<object>> instanceContainer in Instances)
            {
                foreach (object o in instanceContainer.Value)
                {
                    if (o is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            Instances.Clear();
        }

        /// <summary>
        /// Gets a LIST of object instances by the registered type
        /// </summary>
        /// <param name="serviceType">The registration type for the object to retrieve</param>
        /// <returns>A LIST of object instances that are part of the type registration</returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            string name = serviceType.FullName;

            Debug.WriteLine("Resolving: " + name);

            return Instances.TryGetValue(serviceType, out List<object> instances) ? instances : new List<object>();
        }
    }
}