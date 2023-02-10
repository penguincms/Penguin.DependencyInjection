using System;
using System.Collections;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to return a list of objects without constructing the list until it is accessed
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public class LazyLoadCollection<T> : IEnumerable<T>
    {
        internal virtual List<T> BackingObject
        {
            get
            {
                LoadedObject ??= LoadMe.Invoke() ?? new List<T>();

                return LoadedObject;
            }
        }

        internal virtual List<T> LoadedObject { get; set; }

        internal virtual Func<List<T>> LoadMe { get; set; }

        /// <summary>
        /// Constructs a new instance of this object
        /// </summary>
        /// <param name="loadingMethod">A Func that should be called on access to return a list of the provided type</param>
        public LazyLoadCollection(Func<List<T>> loadingMethod)
        {
            LoadMe = loadingMethod;
        }

        /// <summary>
        /// Returns the underlying enumerator for the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return BackingObject.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return BackingObject.GetEnumerator();
        }
    }
}