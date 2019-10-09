using System;

namespace Penguin.DependencyInjection.Interfaces
{
    internal interface IDeferredResolutionCollection
    {
        void Add<T>(Func<T> resolution) where T : class;
    }
}