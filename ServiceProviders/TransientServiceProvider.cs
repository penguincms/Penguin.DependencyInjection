using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Penguin.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// This class causes a new instance of the object to be created every time it is requested
    /// </summary>
    public class TransientServiceProvider : StaticServiceProvider
    {
        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="t">Not used</param>
        /// <param name="o">Not used</param>
        public override void Add(Type t, object o)
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="serviceType">Not used</param>
        /// <returns>Not used</returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            string name = serviceType.FullName;

            Debug.WriteLine("Resolving: " + name);

            return new List<object>();
        }
    }
}