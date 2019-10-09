using Penguin.DependencyInjection.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Penguin.DependencyInjection.Objects
{
    internal class DeferredResolutionCollection<TItem> : IEnumerable<TItem>, IDeferredResolutionCollection where TItem : class
    {
        internal class DeferredResolutionItem
        {
            internal enum DeferredResolutionState
            {
                Unresolved,
                Resolving,
                Resolved
            }

            public DeferredResolutionState State { get; private set; } = DeferredResolutionState.Unresolved;

            private Func<TItem> Resolution;

            private TItem Value;

            internal DeferredResolutionItem(Func<TItem> resolution)
            {
                Resolution = resolution;
            }

            public TItem GetValue()
            {
                switch (State)
                {
                    case DeferredResolutionState.Resolving:
                        throw new Exception("The internal object is already resolving and can not be called again on this stack until resolution is completed");
                    case DeferredResolutionState.Resolved:
                        return Value;

                    case DeferredResolutionState.Unresolved:
                        State = DeferredResolutionState.Resolving;
                        Value = Resolution();
                        State = DeferredResolutionState.Resolved;
                        Resolution = null;
                        return Value;

                    default:
                        throw new Exception($"Unknown resolution state {State}");
                }
            }
        }

        private IList<DeferredResolutionItem> DeferredResolvers { get; set; } = new List<DeferredResolutionItem>();

        public void Add(Func<TItem> resolution)
        {
            DeferredResolvers.Add(new DeferredResolutionItem(resolution));
        }

        void IDeferredResolutionCollection.Add<T>(Func<T> resolution)
        {
            this.Add(() => resolution.Invoke() as TItem);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (DeferredResolutionItem deferredResolutionItem in DeferredResolvers)
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

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}