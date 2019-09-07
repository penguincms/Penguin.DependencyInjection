using System;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to return an IEnumerable of objects without constructing the list until it is accessed
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public class LazyIEnumerable<T> where T : class
    {
        /// <summary>
        /// Constructs a new instance of this object
        /// </summary>
        /// <param name="loadingMethod">A Func that should be called on access to return an IEnumerable of the provided type</param>
        public LazyIEnumerable(Func<IEnumerable<T>> loadingMethod)
        {
            this.LoadMe = loadingMethod;
        }

        internal virtual IEnumerable<T> _backingObject { get; set; }

        internal virtual IEnumerable<T> BackingObject
        {
            get
            {
                if (this._backingObject == null)
                {
                    this._backingObject = this.LoadMe.Invoke() ?? new List<T>();
                }

                return this._backingObject;
            }
        }

        internal virtual Func<IEnumerable<T>> LoadMe { get; set; }
    }
}