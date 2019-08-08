using System;

namespace Penguin.DependencyInjection.Attributes
{
    /// <summary>
    /// Allows attributing a class so that the DI automatically registers it to itself with the given Service Provider type
    /// </summary>
    public class ServiceProviderAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// The type of the service provider that should be used to resolve this class
        /// </summary>
        public Type ServiceProvider { get; set; }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructs a new instance of this attribute
        /// </summary>
        /// <param name="serviceProvider">The type of the service provider that should be used to resolve this class</param>
        public ServiceProviderAttribute(Type serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        #endregion Constructors
    }
}