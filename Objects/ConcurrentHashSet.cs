using System;
using System.Collections.Generic;
using System.Threading;

namespace Penguin.DependencyInjection.Objects
{
    internal class ConcurrentHashSet<T> : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly HashSet<T> _hashSet = new HashSet<T>();

        #region Implementation of ICollection<T> ...ish
        public bool Add(T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                return this._hashSet.Add(item);
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        public void Clear()
        {
            this._lock.EnterWriteLock();
            try
            {
                this._hashSet.Clear();
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        public bool Contains(T item)
        {
            this._lock.EnterReadLock();
            try
            {
                return this._hashSet.Contains(item);
            }
            finally
            {
                if (this._lock.IsReadLockHeld)
                {
                    this._lock.ExitReadLock();
                }
            }
        }

        public bool Remove(T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                return this._hashSet.Remove(item);
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        public int Count
        {
            get
            {
                this._lock.EnterReadLock();
                try
                {
                    return this._hashSet.Count;
                }
                finally
                {
                    if (this._lock.IsReadLockHeld)
                    {
                        this._lock.ExitReadLock();
                    }
                }
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._lock != null)
                {
                    this._lock.Dispose();
                }
            }
        }
        ~ConcurrentHashSet()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
