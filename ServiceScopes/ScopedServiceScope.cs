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
        #region Properties

        /// <summary>
        /// The Scoped Service Provider that will be handling this Scope
        /// </summary>
        public ScopedServiceProvider RequestProvider { get; set; }

        /// <summary>
        /// The Dependency Engine that will be handling this scope
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        #endregion Properties

        #region Constructors

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

        #endregion Constructors

        #region Methods

        /// <summary>
        /// This does nothing because it should all be handled by GC
        /// </summary>
        public void Dispose()
        {
        }

        #endregion Methods
    }
}