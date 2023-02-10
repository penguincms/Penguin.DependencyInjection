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
        /// <summary>
        /// The dependency consolidators instantiated as part of this resolution stack
        /// </summary>
        internal Dictionary<Type, object> DependencyConsolidators { get; } = new Dictionary<Type, object>();

        /// <summary>
        /// A registration stack optionally used to detect circular references
        /// </summary>
        internal Stack<Registration> RegistrationStack { get; } = new Stack<Registration>();

        internal IDictionary<Type, object> ResolutionPackageServices { get; } = new Dictionary<Type, object>();

        /// <summary>
        /// The collection of service providers to be used
        /// </summary>
        internal IDictionary<Type, AbstractServiceProvider> ServiceProviders { get; }

        /// <summary>
        /// Constructs a new instance of the resolution package
        /// </summary>
        /// <param name="serviceProviders"></param>
        public ResolutionPackage(IDictionary<Type, AbstractServiceProvider> serviceProviders)
        {
            ServiceProviders = serviceProviders;
            ResolutionPackageServices.Add(GetType(), this);
        }

        internal void AddStack(Registration match)
        {
            if (Engine.DetectCircularResolution)
            {
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
                _ = RegistrationStack.Pop();
            }
        }
    }
}