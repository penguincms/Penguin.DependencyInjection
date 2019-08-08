using System;
using System.Collections;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to return a list of objects without constructing the list until it is accessed
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public class LazyList<T> : IEnumerable<T>
    {
        #region Constructors

        /// <summary>
        /// Constructs a new instance of this object
        /// </summary>
        /// <param name="loadingMethod">A Func that should be called on access to return a list of the provided type</param>
        public LazyList(Func<List<T>> loadingMethod)
        {
            this.LoadMe = loadingMethod;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Returns the underlying enumerator for the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => this.BackingObject.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.BackingObject.GetEnumerator();

        #endregion Methods

        #region Properties

        internal virtual List<T> _backingObject { get; set; }

        internal virtual List<T> BackingObject
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

        internal virtual Func<List<T>> LoadMe { get; set; }

        #endregion Properties
    }
}