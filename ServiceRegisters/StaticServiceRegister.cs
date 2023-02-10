using Penguin.DependencyInjection.Abstractions.Enums;
using Penguin.DependencyInjection.Abstractions.Interfaces;
using Penguin.DependencyInjection.ServiceProviders;
using System;

namespace Penguin.DependencyInjection.ServiceRegisters
{
    internal class StaticServiceRegister : IServiceRegister
    {
        public void Register<TRegistration, TImplementation>(ServiceLifetime serviceLifetime) where TImplementation : TRegistration
        {
            Engine.Register<TRegistration, TImplementation>(GetServiceProvider(serviceLifetime));
        }

        public void Register(Type TRegistration, Type TImplementation, ServiceLifetime serviceLifetime)
        {
            Engine.Register(TRegistration, TImplementation, GetServiceProvider(serviceLifetime));
        }

        public void Register<TRegistration>(Func<IServiceProvider, TRegistration> RetrieveInstance, ServiceLifetime serviceLifetime)
        {
            Engine.Register<TRegistration>(RetrieveInstance, GetServiceProvider(serviceLifetime));
        }

        public void Register(Type TRegistration, Func<IServiceProvider, object> RetrieveInstance, ServiceLifetime serviceLifetime)
        {
            Engine.Register(TRegistration, RetrieveInstance, GetServiceProvider(serviceLifetime));
        }

        internal static Type GetServiceProvider(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Scoped => typeof(ScopedServiceProvider),
                ServiceLifetime.Singleton => typeof(SingletonServiceProvider),
                ServiceLifetime.Transient => typeof(TransientServiceProvider),
                _ => throw new Exception($"Service provider for lifetime {lifetime} not mapped"),
            };
        }
    }
}