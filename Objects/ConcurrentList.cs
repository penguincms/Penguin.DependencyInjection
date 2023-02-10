using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// A Concurrent List object
    /// </summary>
    /// <typeparam name="T">Any collection type</typeparam>
    public class ConcurrentList<T> : IList<T>
    {
        /// <summary>
        /// Returns a count of the items in the collection
        /// </summary>
        public int Count => ((IList<T>)Backing).Count;

        /// <summary>
        /// This is not read only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// The backing object for this collection
        /// </summary>
        protected List<T> Backing { get; }

        private object ListLock { get; set; }

        /// <summary>
        /// Constucts a new instance of this class
        /// </summary>
        public ConcurrentList()
        {
            Backing = new List<T>();
            ListLock = new object();
        }

        /// <summary>
        /// Gets/Sets the item at the given index
        /// </summary>
        /// <param name="index">The index of the item to set/get</param>
        /// <returns>The item at that index</returns>
        public T this[int index]
        {
            get
            {
                T result;
                lock (ListLock)
                {
                    result = ((IList<T>)Backing)[index];
                }
                return result;
            }
            set
            {
                lock (ListLock)
                {
                    ((IList<T>)Backing)[index] = value;
                }
            }
        }

        /// <summary>
        /// Adds a new object to this collection
        /// </summary>
        /// <param name="item">The object to add</param>
        public void Add(T item)
        {
            lock (ListLock)
            {
                Backing.Add(item);
            }
        }

        /// <summary>
        /// Clears the backing list
        /// </summary>
        public void Clear()
        {
            lock (ListLock)
            {
                ((IList<T>)Backing).Clear();
            }
        }

        /// <summary>
        /// Checks if the list contains the item
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>True if the item exists in the list</returns>
        public bool Contains(T item)
        {
            bool result;
            lock (ListLock)
            {
                result = ((IList<T>)Backing).Contains(item);
            }
            return result;
        }

        /// <summary>
        /// Copies to the array
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The index to start the copy at</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (ListLock)
            {
                ((IList<T>)Backing).CopyTo(array, arrayIndex);
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
                enumerator = Backing.ToList().GetEnumerator();
            }
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerator<T> enumerator;
            lock (ListLock)
            {
                enumerator = Backing.ToList().GetEnumerator();
            }
            return enumerator;
        }

        /// <summary>
        /// Returns the index of the item
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>The index of the item</returns>
        public int IndexOf(T item)
        {
            int i;
            lock (ListLock)
            {
                i = ((IList<T>)Backing).IndexOf(item);
            }
            return i;
        }

        /// <summary>
        /// Inserts an item into the collection
        /// </summary>
        /// <param name="index">The index to insert to</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, T item)
        {
            lock (ListLock)
            {
                ((IList<T>)Backing).Insert(index, item);
            }
        }

        /// <summary>
        /// Removes an object from this list
        /// </summary>
        /// <param name="o">The object to remove</param>
        public void Remove(T o)
        {
            lock (ListLock)
            {
                _ = Backing.Remove(o);
            }
        }

        /// <summary>
        /// Removes an item from the list
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if it was removed successfully</returns>
        bool ICollection<T>.Remove(T item)
        {
            bool result;
            lock (ListLock)
            {
                result = ((IList<T>)Backing).Remove(item);
            }
            return result;
        }

        /// <summary>
        /// Removes the item at the index
        /// </summary>
        /// <param name="index">The idex of the item to remove</param>
        public void RemoveAt(int index)
        {
            lock (ListLock)
            {
                ((IList<T>)Backing).RemoveAt(index);
            }
        }
    }
}