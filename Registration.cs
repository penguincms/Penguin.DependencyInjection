using Penguin.DependencyInjection.ServiceProviders;
using System;

namespace Penguin.DependencyInjection
{
    /// <summary>
    /// Defines a single registration of types for the Dependency Engine
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// A Func to be used when creating the instance
        /// </summary>
        public Func<IServiceProvider, object> InjectionFactory { get => this._InjectionFactory; set => this._InjectionFactory = value; }

        /// <summary>
        /// The "Key" type defining what request type this registration applies to
        /// </summary>
        public Type RegisteredType
        {
            get => this._RegisteredType;
            set
            {
                this._RegisteredType = value;
#if DEBUG
                this.RegisteredTypeName = value?.ToString();
#endif
            }
        }

        /// <summary>
        /// The Type of the service provider that should handle/store instances of this registrations object
        /// </summary>
        public Type ServiceProvider
        {
            get => this._ServiceProvider ?? typeof(TransientServiceProvider);
            set
            {
                this._ServiceProvider = value;
#if DEBUG
                this.ServiceProviderName = value?.ToString();
#endif
            }
        }

        /// <summary>
        /// The "Value" type defining what should be returned when the "Key" (RegisteredType) is requested
        /// </summary>
        public Type ToInstantiate
        {
            get => this._ToInstantiate ?? this._RegisteredType;
            set
            {
                this._ToInstantiate = value;
#if DEBUG
                this.ToInstantiateName = value?.ToString();
#endif
            }
        }

        private Func<IServiceProvider, object> _InjectionFactory { get; set; }
        private Type _RegisteredType { get; set; }
        private Type _ServiceProvider { get; set; }
        private Type _ToInstantiate { get; set; }
#if DEBUG
        public string RegisteredTypeName { get; set; }
        public string ToInstantiateName { get; set; }
        public string ServiceProviderName { get; set; }
#endif
    }
}