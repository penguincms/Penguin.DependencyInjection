using System;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// A base class for the various lazy initializers
    /// </summary>
    /// <typeparam name="T">Any type to return</typeparam>
    public abstract class LazyBase<T> where T : class
    {
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Constructs a new instance of this base class
        /// </summary>
        /// <param name="serviceProvider">The service provider to use as a data source</param>
        protected LazyBase(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
    }
}