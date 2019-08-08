using Penguin.DependencyInjection.ServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.DependencyInjection.Objects
{
    /// <summary>
    /// Used to hold the required objects for resolving through the static methods
    /// </summary>
    public class ResolutionPackage
    {
        #region Properties

        /// <summary>
        /// A registration stack optionally used to detect circular references
        /// </summary>
        public Stack<Registration> RegistrationStack { get; set; } = new Stack<Registration>();

        /// <summary>
        /// The collection of service providers to be used
        /// </summary>
        public IDictionary<Type, AbstractServiceProvider> ServiceProviders { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new instance of the resolution package
        /// </summary>
        /// <param name="serviceProviders"></param>
        public ResolutionPackage(IDictionary<Type, AbstractServiceProvider> serviceProviders)
        {
            ServiceProviders = serviceProviders;
        }

        #endregion Constructors

        #region Methods

        internal void AddStack(Registration match)
        {
            if (Engine.DetectCircularResolution)
            {
                RegistrationStack = RegistrationStack ?? new Stack<Registration>();

                if (!RegistrationStack.Contains(match))
                {
                    RegistrationStack.Push(match);
                }
                else
                {
                    RegistrationStack.Push(match);

                    string toThrow = string.Join(" => ", RegistrationStack.Select(s => $"{s.RegisteredType.Name} : {s.ToInstantiate.Name}"));

                    throw new StackOverflowException(toThrow);
                }
            }
        }

        internal void RemoveStack()
        {
            if (Engine.DetectCircularResolution)
            {
                RegistrationStack.Pop();
            }
        }

        #endregion Methods
    }
}