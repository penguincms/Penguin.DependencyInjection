using Penguin.DependencyInjection.Objects;
using System;
using System.Collections;

namespace Penguin.DependencyInjection
{
    public partial class Engine
    {
        #region Classes

        /// <summary>
        /// Methods that allow for type resolutions using static providers, to remove need to instantiate engine if there are no scoped providers
        /// </summary>
        public static class Static
        {
            #region Methods

            /// <summary>
            /// Attempts to resolve a service using registrations
            /// </summary>
            /// <typeparam name="T">The type of class being requested</typeparam>
            /// <returns>The registered or constructed instance of the class, or null, or error</returns>
            public static T GetService<T>() where T : class => Engine.Resolve(typeof(T), new ResolutionPackage(StaticProviders)) as T;

            /// <summary>
            /// Attempts to resolve a service using registrations
            /// </summary>
            /// <param name="serviceType">The type of class being requested</param>
            /// <returns>The registered or constructed instance of the class, or null, or error</returns>
            public static object GetService(Type serviceType) => Engine.Resolve(serviceType, new ResolutionPackage(StaticProviders));


            /// <summary>
            /// Attempts to resolve an IEnumerable of all registered instances of the type requested
            /// </summary>
            /// <param name="t">The type of class being requested</param>
            /// <returns>The registered or constructed instances of the class, or null, or error</returns>
            public static IEnumerable ResolveMany(Type t) => Engine.ResolveMany(t, new ResolutionPackage(StaticProviders));

            /// <summary>
            /// Returns a single instance of the type based on the LAST registration
            /// </summary>
            /// <param name="t">The type of class being requested</param>
            /// <returns>The registered or constructed instances of the class, or null, or error</returns>
            public static object ResolveSingle(Type t) => Engine.ResolveSingle(t, new ResolutionPackage(StaticProviders));

            #endregion Methods
        }

        #endregion Classes
    }
}