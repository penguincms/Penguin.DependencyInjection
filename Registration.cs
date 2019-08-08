using Penguin.DependencyInjection.ServiceProviders;
using System;

namespace Penguin.DependencyInjection
{
    /// <summary>
    /// Defines a single registration of types for the Dependency Engine
    /// </summary>
    public class Registration
    {
        #region Properties

        /// <summary>
        /// A Func to be used when creating the instance
        /// </summary>
        public Func<IServiceProvider, object> InjectionFactory { get { return _InjectionFactory; } set { _InjectionFactory = value; } }

        /// <summary>
        /// The "Key" type defining what request type this registration applies to
        /// </summary>
        public Type RegisteredType { get { return _RegisteredType; } set { _RegisteredType = value; } }

        /// <summary>
        /// The Type of the service provider that should handle/store instances of this registrations object
        /// </summary>
        public Type ServiceProvider { get { return _ServiceProvider ?? typeof(TransientServiceProvider); } set { _ServiceProvider = value; } }

        /// <summary>
        /// The "Value" type defining what should be returned when the "Key" (RegisteredType) is requested
        /// </summary>
        public Type ToInstantiate { get { return _ToInstantiate ?? _RegisteredType; } set { _ToInstantiate = value; } }


        private Func<IServiceProvider, object> _InjectionFactory { get; set; }
        private Type _RegisteredType { get; set; }
        private Type _ServiceProvider { get; set; }
        private Type _ToInstantiate { get; set; }

        #endregion Properties
    }
}