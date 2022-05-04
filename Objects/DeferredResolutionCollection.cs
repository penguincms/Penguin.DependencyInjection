using Penguin.DependencyInjection.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.Objects
{
    internal class DeferredResolutionCollection<TItem> : IEnumerable<TItem>, IDeferredResolutionCollection where TItem : class
    {
        private IList<DeferredResolutionItem> DeferredResolvers { get; set; } = new List<DeferredResolutionItem>();

        internal class DeferredResolutionItem
        {
            private Func<TItem> Resolution;

            private TItem Value;

            public DeferredResolutionState State { get; private set; } = DeferredResolutionState.Unresolved;

            internal enum DeferredResolutionState
            {
                Unresolved,
                Resolving,
                Resolved
            }

            internal DeferredResolutionItem(Func<TItem> resolution)
            {
                this.Resolution = resolution;
            }

            public TItem GetValue()
            {
                switch (this.State)
                {
                    case DeferredResolutionState.Resolving:
                        throw new Exception("The internal object is already resolving and can not be called again on this stack until resolution is completed");
                    case DeferredResolutionState.Resolved:
                        return this.Value;

                    case DeferredResolutionState.Unresolved:
                        this.State = DeferredResolutionState.Resolving;
                        this.Value = this.Resolution();
                        this.State = DeferredResolutionState.Resolved;
                        this.Resolution = null;
                        return this.Value;

                    default:
                        throw new Exception($"Unknown resolution state {this.State}");
                }
            }
        }

        public void Add(Func<TItem> resolution)
        {
            this.DeferredResolvers.Add(new DeferredResolutionItem(resolution));
        }

        void IDeferredResolutionCollection.Add<T>(Func<T> resolution)
        {
            this.Add(() => resolution.Invoke() as TItem);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (DeferredResolutionItem deferredResolutionItem in this.DeferredResolvers)
            {
                if (deferredResolutionItem.State == DeferredResolutionItem.DeferredResolutionState.Resolving)
                {
                    continue;
                }
                else
                {
                    yield return deferredResolutionItem.GetValue();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}