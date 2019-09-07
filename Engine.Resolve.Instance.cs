using Penguin.DependencyInjection.Objects;
using System;
using System.Collections;

namespace Penguin.DependencyInjection
{
    public partial class Engine
    {
        /// <summary>
        /// Attempts to resolve a service using registrations
        /// </summary>
        /// <param name="serviceType">The type of class being requested</param>
        /// <returns>The registered or constructed instance of the class, or null, or error</returns>
        public object GetService(Type serviceType)
        {
            object toReturn = Resolve(serviceType, new ResolutionPackage(AllProviders));

            return toReturn;
        }

        /// <summary>
        /// Attempts to resolve a service using registrations
        /// </summary>
        /// <param name="t">The type of class being requested</param>
        /// <returns>The registered or constructed instance of the class, or null, or error</returns>
        public object Resolve(Type t) => Engine.Resolve(t, new ResolutionPackage(AllProviders));

        /// <summary>
        /// Attempts to resolve a service using registrations
        /// </summary>
        /// <typeparam name="T">The type of class being requested</typeparam>
        /// <returns>The registered or constructed instance of the class, or null, or error</returns>
        public object Resolve<T>() where T : class => Engine.Resolve(typeof(T), new ResolutionPackage(AllProviders)) as T;

        /// <summary>
        /// Attempts to resolve an IEnumerable of all registered instances of the type requested
        /// </summary>
        /// <param name="t">The type of class being requested</param>
        /// <returns>The registered or constructed instances of the class, or null, or error</returns>
        public IEnumerable ResolveMany(Type t) => Engine.ResolveMany(t, new ResolutionPackage(AllProviders));

        /// <summary>
        /// Attempts to resolve any properties on the given type, that have the Dependency attribute
        /// </summary>
        /// <param name="o">The object to resolve the properties of</param>
        /// <returns>The same object</returns>
        public object ResolveProperties(object o) => Engine.ResolveProperties(o, new ResolutionPackage(AllProviders));

        /// <summary>
        /// Returns a single instance of the type based on the LAST registration
        /// </summary>
        /// <param name="t">The type of class being requested</param>
        /// <returns>The registered or constructed instances of the class, or null, or error</returns>
        public object ResolveSingle(Type t) => Engine.ResolveSingle(t, new ResolutionPackage(AllProviders));
    }
}