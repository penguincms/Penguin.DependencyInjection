﻿using Penguin.Debugging;
using Penguin.DependencyInjection.Abstractions.Interfaces;
using Penguin.DependencyInjection.Interfaces;
using Penguin.DependencyInjection.Objects;
using Penguin.DependencyInjection.ServiceProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                if (!resolutionPackage.ServiceProviders.TryGetValue(typeof(TransientServiceProvider), out thisManager))
                {
                    throw new Exception($"Type {match.ToInstantiate} could not be created because service provider of type {match.ServiceProvider} was not found in the current registrations and a transient service provider could not be found");
                }
                else
                {
                    StaticLogger.Log($"Type {match.ToInstantiate} created using transient service provider because {match.ServiceProvider} was not found in the current registrations", StaticLogger.LoggingLevel.Call);
                }
            }

            //If no registration was found, or there was no instance existing
            if (thisManager.GetService(match.ToInstantiate) is not IList<object> toReturn || !toReturn.Any())
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

        private static T Resolve<T>(ResolutionPackage resolutionPackage) where T : class
        {
            return Resolve(typeof(T), resolutionPackage) as T;
        }

        private static object Resolve(Type t, ResolutionPackage resolutionPackage, bool optional = false)
        {
            if (t is null)
            {
                return null;
            }

            if (IsValidIEnumerable(t, out Type collectionType))
            {
                IList toReturn = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(collectionType));

                foreach (object thisItem in ResolveMany(t, resolutionPackage))
                {
                    _ = toReturn.Add(thisItem);
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
            if (!IsResolvable(t, resolutionPackage))
            {
                return null;
            }

            if (resolutionPackage.ResolutionPackageServices.TryGetValue(t, out object value))
            {
                return value;
            }

            //This whole IF block attempts to find a dependency consolidator for the requested type, and if its found it squishes all the registered instances into it
            //Which effectively allows for a class that converts an IEnumerable of a registered type into a single instance, which is great for things like providers
            //that may be registered independently but can be treated as a single unit through a consolidating class, that way everywhere the dependency is resolved
            //doesn't need to accept an IEnumerable of the types
            if (DependencyConsolidators.TryGetValue(t, out Type consolidatorType))
            {
                if (resolutionPackage.DependencyConsolidators.TryGetValue(t, out object consolidatedObject))
                {
                    return consolidatedObject;
                }

                object consolidator;

                foreach (Registration reg in ResolveType(consolidatorType))
                {
                    if (!resolutionPackage.ServiceProviders.TryGetValue(reg.ServiceProvider, out AbstractServiceProvider provider))
                    {
                        provider = Activator.CreateInstance(reg.ServiceProvider) as AbstractServiceProvider;
                        resolutionPackage.ServiceProviders.Add(reg.ServiceProvider, provider);
                    }

                    consolidator = (provider.GetService(consolidatorType) as IList<object>)?.SingleOrDefault();

                    if (consolidator != null)
                    {
                        return consolidator;
                    }
                }

                try
                {
                    consolidator = Activator.CreateInstance(consolidatorType);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occured or creating and instance of the dependency consolidator type {consolidatorType}", ex);
                }

                IDeferredResolutionCollection deferredResolutionCollection = Activator.CreateInstance(typeof(DeferredResolutionCollection<>).MakeGenericType(t)) as IDeferredResolutionCollection;

                foreach (Registration reg in ResolveType(t))
                {
                    if (reg.ToInstantiate != consolidatorType)
                    {
                        deferredResolutionCollection.Add(() => Resolve(reg, resolutionPackage, optional).LastOrDefault());
                    }
                }

                MethodInfo setMethod = typeof(IConsolidateDependencies<>).MakeGenericType(t).GetMethod(nameof(IConsolidateDependencies<object>.Consolidate));

                consolidatedObject = setMethod.Invoke(consolidator, new object[] { deferredResolutionCollection });

                resolutionPackage.DependencyConsolidators.Add(t, consolidatedObject);

                foreach (Registration reg in ResolveType(consolidatorType))
                {
                    resolutionPackage.ServiceProviders[reg.ServiceProvider].Add(consolidatorType, consolidator);
                }

                return consolidatedObject;
            }
            else
            {
                foreach (Registration reg in ResolveType(t))
                {
                    object toReturn = Resolve(reg, resolutionPackage, optional).LastOrDefault();

                    if (toReturn != null)
                    {
                        return toReturn;
                    }
                }
            }

            return null;
        }
    }
}