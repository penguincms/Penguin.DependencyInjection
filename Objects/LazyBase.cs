using System;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// A base class for the various lazy initializers
    /// </summary>
    /// <typeparam name="T">Any type to return</typeparam>
    public abstract class LazyBase<T> where T : class
    {
        /// <summary>
        /// Constructs a new instance of this base class
        /// </summary>
        /// <param name="serviceProvider">The service provider to use as a data source</param>
        public LazyBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        internal IServiceProvider ServiceProvider { get; set; }
    }
}