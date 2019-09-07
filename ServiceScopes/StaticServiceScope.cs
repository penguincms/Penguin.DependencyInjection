using Microsoft.Extensions.DependencyInjection;
using System;

namespace Penguin.DependencyInjection.ServiceScopes
{
    /// <summary>
    /// A service scope for resolving dependencies in a static manner
    /// </summary>
    public class StaticServiceScope : IServiceScope
    {
        /// <summary>
        /// Unused
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Creates a new instance of this service scope.
        /// </summary>
        public StaticServiceScope()
        {
            if (Engine is null)
            {
                Engine = new Engine();
            }

            ServiceProvider = Engine;
        }

        /// <summary>
        /// Unused
        /// </summary>
        public void Dispose()
        {
        }

        private static Engine Engine { get; set; }
    }
}