﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// A STATIC service provider that returns the same object instance across all DI instances
    /// </summary>
    public class SingletonServiceProvider : StaticServiceProvider
    {
        /// <summary>
        /// A static list of the current instances held by this provider
        /// </summary>
        public static ConcurrentDictionary<Type, List<object>> Instances { get; } = new ConcurrentDictionary<Type, List<object>>();

        /// <summary>
        /// Add a new instance to the provider
        /// </summary>
        /// <param name="t">The type that maps to this instance</param>
        /// <param name="o">The actual instance</param>
        public override void Add(Type t, object o)
        {
            if (Instances.TryGetValue(t, out List<object> instances))
            {
                instances.Add(o);
            }
            else
            {
                _ = Instances.TryAdd(t, new List<object>() { o });
            }
        }

        /// <summary>
        /// Gets an object by the registered type
        /// </summary>
        /// <param name="serviceType">The type of registration to return</param>
        /// <returns>The object registered to that type</returns>
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