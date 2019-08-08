using Penguin.DependencyInjection.Objects;
using Penguin.DependencyInjection.ServiceProviders;
using Penguin.Reflection.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.DependencyInjection
{
    /// <summary>
    /// The core dependency engine. Instantiation needed to support Scoped service providers
    /// </summary>
    public partial class Engine
    {
        #region Methods

        private static IEnumerable<object> Resolve(Registration match, ResolutionPackage resolutionPackage, bool optional = false)
        {
            resolutionPackage.AddStack(match);

            IList<object> toReturn = null;

            //We resolve with that registration. Or attempt to
            AbstractServiceProvider thisManager = null;

            if (resolutionPackage.ServiceProviders.ContainsKey(match.ServiceProvider))
            {
                thisManager = resolutionPackage.ServiceProviders[match.ServiceProvider];
            }
            //Anything where the service provider is not registered, is a singleton
            //This should only effect scoped providers when they're out of scope.
            else if (match.ServiceProvider != null)
            {
                thisManager = new SingletonServiceProvider();
            }

            toReturn = thisManager.GetService(match.ToInstantiate) as IList<object>;
            //If no registration was found, or there was no instance existing
            if (toReturn is null || !toReturn.Any())
            {
                //Create an instance
                toReturn = new List<object>();

                object o = CreateRegisteredInstance(match, resolutionPackage, optional);

                if (o != null)
                {
                    thisManager?.Add(match.ToInstantiate, o);
                    toReturn.Add(o);
                }
            }

            resolutionPackage.RemoveStack();

            return toReturn;
        }

        private static T Resolve<T>(ResolutionPackage resolutionPackage) where T : class => Resolve(typeof(T), resolutionPackage) as T;

        private static object Resolve(Type t, ResolutionPackage resolutionPackage, bool optional = false)
        {
            if (t is null)
            {
                return null;
            }

            string name = t.Name;

            Type collectionType = t.GetCollectionType();
            Type listType = null;

            if (collectionType != null)
            {
                listType = typeof(List<>).MakeGenericType(collectionType);
            }

            if (listType != null && t.IsAssignableFrom(listType))
            {
                IList toReturn = (IList)Activator.CreateInstance(listType);

                foreach (object thisItem in ResolveMany(t, resolutionPackage))
                {
                    toReturn.Add(thisItem);
                }

                return toReturn;
            }
            else
            {
                return ResolveSingle(t, resolutionPackage, optional);
            }
        }

        private static IEnumerable ResolveMany(Type t, ResolutionPackage resolutionPackage)
        {
            SynchronizedCollection<Registration> matchList = ResolveType(t);

            foreach (Registration match in matchList)
            {
                foreach (object instance in Resolve(match, resolutionPackage))
                {
                    yield return instance;
                }
            }
        }

        /// <summary>
        /// Returns a single instance of the type based on the LAST registration
        /// </summary>
        /// <param name="t">The type to return</param>
        /// <param name="resolutionPackage">A resolution package containing any providers</param>
        /// <param name="optional">Dont throw an error if its null</param>
        /// <returns>An instance of the requested type, if registered</returns>
        private static object ResolveSingle(Type t, ResolutionPackage resolutionPackage, bool optional = false)
        {
            if (!IsResolvable(t))
            {
                return null;
            }

            List<Registration> matchList = ResolveType(t).ToList();

            foreach (Registration reg in matchList)
            {
                object toReturn = Resolve(reg, resolutionPackage, optional).LastOrDefault();

                if (toReturn != null)
                {
                    return toReturn;
                }
            }

            return null;
        }

        #endregion Methods
    }
}