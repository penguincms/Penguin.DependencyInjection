using Penguin.DependencyInjection.Abstractions.Interfaces;
using System.Diagnostics.Contracts;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// A base class for any service providers that should be registered as static instances with the dependency injector,
    /// since it makes no sense to force registrations at construction for providers backed by static fields.
    /// </summary>
    public abstract class StaticServiceProvider : AbstractServiceProvider, ISelfRegistering
    {
        /// <summary>
        /// Registeres this instance with the dependency injector
        /// </summary>
        /// <param name="engine">The dependency injector to register</param>
        public virtual void Register(Engine engine)
        {
            if (engine is null)
            {
                throw new System.ArgumentNullException(nameof(engine));
            }

            engine.Register(this);
        }
    }
}