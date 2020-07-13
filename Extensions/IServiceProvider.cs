using System;
using System.Diagnostics.Contracts;

namespace Penguin.DependencyInjection.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class IServiceProviderExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Returns a service from a service provider by casting it to a requested type
        /// </summary>
        /// <typeparam name="T">Type to request</typeparam>
        /// <param name="provider">The service provider to use as a source</param>
        /// <returns>Any resolved object casted to the requested type</returns>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetService(typeof(T)) as T;
        }
    }
}