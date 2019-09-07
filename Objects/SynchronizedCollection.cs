using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// A Concurrent List object
    /// </summary>
    /// <typeparam name="T">Any collection type</typeparam>
    public class SynchronizedCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Constucts a new instance of this class
        /// </summary>
        public SynchronizedCollection()
        {
            _backing = new List<T>();
            ListLock = new object();
        }

        /// <summary>
        /// Adds a new object to this collection
        /// </summary>
        /// <param name="o">The object to add</param>
        public void Add(T o)
        {
            lock (ListLock)
            {
                _backing.Add(o);
            }
        }

        /// <summary>
        /// Returns a thread safe enumerator
        /// </summary>
        /// <returns>A thread safe enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            IEnumerator<T> enumerator;
            lock (ListLock)
            {
                enumerator = _backing.ToList().GetEnumerator();
            }
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerator<T> enumerator;
            lock (ListLock)
            {
                enumerator = _backing.ToList().GetEnumerator();
            }
            return enumerator;
        }

        /// <summary>
        /// Removes an object from this list
        /// </summary>
        /// <param name="o">The object to remove</param>
        public void Remove(T o)
        {
            lock (ListLock)
            {
                _backing.Remove(o);
            }
        }

        /// <summary>
        /// The backing object for this collection
        /// </summary>
        protected List<T> _backing { get; set; }

        private object ListLock { get; set; }
    }
}