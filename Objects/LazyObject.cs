﻿using Penguin.DependencyInjection.Extensions;
using System;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to provide an object that created until it is accessed
    /// </summary>
    /// <typeparam name="T">The type of object to provide when accessed</typeparam>
    public class LazyObject<T> : LazyBase<T> where T : class
    {
        #region Properties

        /// <summary>
        /// Returns an object from the provided factory/provider 
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (!this.Loaded)
                {
                    this._value = this.LoadMe.Invoke();
                }

                return this._value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new instance of this object using the Func as the data source
        /// </summary>
        /// <param name="loadingMethod">A func that returns an object of the requested type</param>
        public LazyObject(Func<T> loadingMethod) : base(null)
        {
            this.LoadMe = loadingMethod;
        }

        /// <summary>
        /// Constructs a new instance of this object using a service provider as the data source
        /// </summary>
        /// <param name="serviceProvider">A service provider configured to return an instance of the requested object</param>
        public LazyObject(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.LoadMe = new Func<T>(() => { return ServiceProvider.GetService<T>(); });
        }

        #endregion Constructors

        internal virtual T _value { get; set; }

        internal virtual Func<T> LoadMe { get; set; }

        private bool Loaded { get; set; }
    }
}