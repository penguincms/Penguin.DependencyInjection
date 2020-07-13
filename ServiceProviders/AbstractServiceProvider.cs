using System;
using System.Diagnostics.Contracts;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// Root service provider class for classes that act as object containers for the dependency injector
    /// </summary>
    public abstract class AbstractServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Adds an instance of the type to this container, using the specified type as the key
        /// </summary>
        /// <param name="t">The type you will be resolving</param>
        /// <param name="o">The object to resolve to</param>
        public abstract void Add(Type t, object o);

        /// <summary>
        /// Adds an instance of the type to this container using the objects type as a key
        /// </summary>
        /// <param name="o">The object to resolve to</param>
        public void Add(object o)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            this.Add(o.GetType(), o);
        }

        /// <summary>
        /// Used to return any objects matching the specified type, from the container
        /// </summary>
        /// <param name="t">The type to use as the Key</param>
        /// <returns>A List of objects (as an object) that match the Key. Cast the list to access the contents</returns>
        public abstract object GetService(Type t);
    }
}