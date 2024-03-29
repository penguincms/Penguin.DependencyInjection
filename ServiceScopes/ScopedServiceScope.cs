﻿using Microsoft.Extensions.DependencyInjection;
using Penguin.DependencyInjection.ServiceProviders;
using System;

namespace Penguin.DependencyInjection.ServiceScopes
{
    /// <summary>
    /// Represents a single scope for DI injection
    /// </summary>
    public class ScopedServiceScope : IServiceScope
    {
        private bool disposedValue;

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
            Engine engine = new();

            RequestProvider = new ScopedServiceProvider();

            engine.Register(RequestProvider);

            RequestProvider.Add(engine);
            RequestProvider.Add(typeof(IServiceProvider), engine);

            ServiceProvider = engine;
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Disposes of the items in the scope
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the items in the service scope
        /// </summary>
        /// <param name="disposing">True if clear managed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RequestProvider.Clear();
                    RequestProvider = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // To detect redundant calls

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ScopedServiceScope()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }
        /// <summary>
        /// Disposes of the items in the scope
        /// </summary>
        ~ScopedServiceScope()
        {
            Dispose(false);
        }
    }
}