using System;

namespace Penguin.DependencyInjection.Attributes
{
    /// <summary>
    /// Marks a property as being injectable. Property remains null if not registered
    /// </summary>
    public sealed class DependencyAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Can be used to specify a default registration type if the property is not registered when resolved
        /// </summary>
        public Type Default { get; set; }

        /// <summary>
        /// I dont actually know what the intent of this was so dont use it
        /// </summary>
        public bool FindService { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates an instance of the attribute instructing the injector to use a specified type if this property type is unregistered
        /// </summary>
        /// <param name="defaultType">The type to resolve this property to if no registrations are found</param>
        public DependencyAttribute(Type defaultType) : this(true)
        {
            Default = defaultType;
        }

        /// <summary>
        /// Creates a default instance of this attribute
        /// </summary>
        /// <param name="findService">Dont set this because I dont know what it does</param>
        public DependencyAttribute(bool findService = false)
        {
            FindService = findService;
        }

        #endregion Constructors
    }
}