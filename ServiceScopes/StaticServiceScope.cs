using Microsoft.Extensions.DependencyInjection;
using System;

namespace Penguin.DependencyInjection.ServiceScopes
{
    /// <summary>
    /// A service scope for resolving dependencies in a static manner
    /// </summary>
    public class StaticServiceScope : IServiceScope
    {
        private bool disposedValue;

        /// <summary>
        /// Unused
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        private static Engine Engine { get; set; }

        /// <summary>
        /// Creates a new instance of this service scope.
        /// </summary>
        public StaticServiceScope()
        {
            Engine ??= new Engine();

            ServiceProvider = Engine;
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Unused
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Unused
        /// </summary>
        /// <param name="disposing">Unused</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // To detect redundant calls

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StaticServiceScope()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }
        /// <summary>
        /// Unused
        /// </summary>
        ~StaticServiceScope()
        {
            Dispose(false);
        }
    }
}