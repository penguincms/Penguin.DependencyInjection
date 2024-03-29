﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to allow access to a dependency that isn't injected until its accessed
    /// </summary>
    /// <typeparam name="T">The type of the dependency to return</typeparam>
    public class LazyDependency<T> : LazyBase<T> where T : class
    {
        private bool IsLoaded;

        private T value;

        /// <summary>
        /// Returns a constructed instance of the dependency
        /// </summary>
        public T Value
        {
            get
            {
                if (!IsLoaded)
                {
                    value = ServiceProvider.GetService<T>();
                    IsLoaded = true;
                }

                return value;
            }
            set
            {
                IsLoaded = true;

                this.value = value;
            }
        }

        /// <summary>
        /// Constructs a new instance of this object
        /// </summary>
        /// <param name="serviceProvider">The service provider to use when returning the dependency</param>
        public LazyDependency(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}