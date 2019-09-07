using Microsoft.Extensions.DependencyInjection;
using System;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to allow access to a dependency that isn't injected until its accessed
    /// </summary>
    /// <typeparam name="T">The type of the dependency to return</typeparam>
    public class LazyDependency<T> : LazyBase<T> where T : class
    {
        /// <summary>
        /// Returns a constructed instance of the dependency
        /// </summary>
        public T Value
        {
            get
            {
                if (!this.IsLoaded)
                {
                    this._Value = ServiceProvider.GetService<T>();
                    this.IsLoaded = true;
                }

                return this._Value;
            }
            set
            {
                this.IsLoaded = true;

                this._Value = value;
            }
        }

        /// <summary>
        /// Constructs a new instance of this object
        /// </summary>
        /// <param name="serviceProvider">The service provider to use when returning the dependency</param>
        public LazyDependency(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private T _Value { get; set; }

        private bool IsLoaded { get; set; }
    }
}