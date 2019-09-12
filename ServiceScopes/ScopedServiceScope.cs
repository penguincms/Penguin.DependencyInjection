using Microsoft.Extensions.DependencyInjection;
using Penguin.DependencyInjection.ServiceProviders;
using System;

namespace Penguin.DependencyInjection.ServiceScopes
{
    /// <summary>
    /// Represents a single scope for DI injection
    /// </summary>
    public class ScopedServiceScope : IServiceScope
    {
        /// <summary>
        /// The Scoped Service Provider that will be handling this Scope
        /// </summary>
        public ScopedServiceProvider RequestProvider { get; set; }

        /// <summary>
        /// The Dependency Engine that will be handling this scope
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Constructs a new instance of this scope, and sets the internal DI to a new instance with a registered scope provider
        /// </summary>
        public ScopedServiceScope()
        {
            Engine engine = new Engine();

            RequestProvider = new ScopedServiceProvider();

            engine.Register(RequestProvider);

            RequestProvider.Add(engine);

            ServiceProvider = engine;
        }

        /// <summary>
        /// This disposes of the scoped objects
        /// </summary>
        public void Dispose()
        {
            RequestProvider.Clear();
            RequestProvider = null;
        }
    }
}