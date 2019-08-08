using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// A STATIC service provider that returns the same object instance across all DI instances
    /// </summary>
    public class SingletonServiceProvider : StaticServiceProvider
    {
        #region Properties

        /// <summary>
        /// A static list of the current instances held by this provider
        /// </summary>
        public static ConcurrentDictionary<Type, List<object>> Instances { get; set; } = new ConcurrentDictionary<Type, List<object>>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add a new instance to the provider
        /// </summary>
        /// <param name="t">The type that maps to this instance</param>
        /// <param name="o">The actual instance</param>
        public override void Add(Type t, object o)
        {
            if (!Instances.ContainsKey(t))
            {
                Instances.TryAdd(t, new List<object>());
            }

            Instances[t].Add(o);
        }

        /// <summary>
        /// Gets an object by the registered type
        /// </summary>
        /// <param name="t">The type of registration to return</param>
        /// <returns>The object registered to that type</returns>
        public override object GetService(Type t)
        {
            return Instances.ContainsKey(t) ? Instances[t] : new List<object>();
        }

        #endregion Methods
    }
}