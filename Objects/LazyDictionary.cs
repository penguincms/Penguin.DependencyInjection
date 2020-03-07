using System;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Attempts to return a dictionary without constructing the list until it is accessed
    /// </summary>
    /// <typeparam name="TKey">The Key Type of the dictionary</typeparam>
    /// <typeparam name="TValue">The Value Type of the dictionary</typeparam>
    public class LazyDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class
    {
        private readonly Func<TKey, TValue> loadMe;

        /// <summary>
        /// Constructs a new instance of this object
        /// </summary>
        /// <param name="loadingMethod">A func that returns a constructed instance of the underlying type</param>
        public LazyDictionary(Func<TKey, TValue> loadingMethod)
        {
            this.loadMe = loadingMethod;
        }

        /// <summary>
        /// Returns a value from the dictionary based on the key
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>The value associated with the key</returns>
        public new TValue this[TKey key] => this.GetValue(key);

        /// <summary>
        /// Returns a value from the dictionary based on the key
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>The value associated with the key</returns>
        protected virtual TValue GetValue(TKey key)
        {
            if (!this.ContainsKey(key))
            {
                TValue toReturn = this.loadMe.Invoke(key);

                if (toReturn == null)
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    this.Add(key, toReturn);
                    return toReturn;
                }
            }
            else
            {
                return base[key];
            }
        }
    }
}