using Penguin.DependencyInjection.Objects;
using Penguin.DependencyInjection.ServiceProviders;
using Penguin.Reflection.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Penguin.Debugging;

namespace Penguin.DependencyInjection
{
    /// <summary>
    /// The core dependency engine. Instantiation needed to support Scoped service providers
    /// </summary>
    public partial class Engine
    {
        private static IEnumerable<object> Resolve(Registration match, ResolutionPackage resolutionPackage, bool optional = false)
        {
            resolutionPackage.AddStack(match);

            //We resolve with that registration. Or attempt to
            if (!resolutionPackage.ServiceProviders.TryGetValue(match.ServiceProvider, out AbstractServiceProvider thisManager))
            {

                if(!resolutionPackage.ServiceProviders.TryGetValue(typeof(TransientServiceProvider), out thisManager)) {
                    throw new Exception($"Type {match.ToInstantiate} could not be created because service provider of type {match.ServiceProvider} was not found in the current registrations and a transient service provider could not be found");
                } else
                {
                    StaticLogger.Log($"Type {match.ToInstantiate} created using transient service provider because {match.ServiceProvider} was not found in the current registrations", StaticLogger.LoggingLevel.Call);
                }
            }

            //If no registration was found, or there was no instance existing
            if (!(thisManager.GetService(match.ToInstantiate) is IList<object> toReturn) || !toReturn.Any())
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
            foreach (Registration match in ResolveType(t))
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

            foreach (Registration reg in ResolveType(t))
            {
                object toReturn = Resolve(reg, resolutionPackage, optional).LastOrDefault();

                if (toReturn != null)
                {
                    return toReturn;
                }
            }

            return null;
        }
    }
}