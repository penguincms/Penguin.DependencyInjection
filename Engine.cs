﻿using Penguin.Debugging;
using Penguin.DependencyInjection.Abstractions;
using Penguin.DependencyInjection.Attributes;
using Penguin.DependencyInjection.Exceptions;
using Penguin.DependencyInjection.Objects;
using Penguin.DependencyInjection.ServiceProviders;
using Penguin.Reflection;
using Penguin.Reflection.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Penguin.DependencyInjection
{
    public partial class Engine : IServiceProvider
    {
        /// <summary>
        /// If true, maintains a resolution stack to attempt to prevent a stack overflow. Likely incurs a performance penalty. Useful for debugging. Default false.
        /// </summary>
        public static bool DetectCircularResolution { get; set; } = false;

        /// <summary>
        /// Whitelists a list of assemblies through the Reflection TypeFactory and then grabs all types and attempts to register any types that are relevant to the
        /// engine
        /// </summary>
        static Engine()
        {
            StaticLogger.Log($"Penguin.DependencyInjection: {Assembly.GetExecutingAssembly().GetName().Version}", StaticLogger.LoggingLevel.Call);

            ChildDependancies = new ConcurrentDictionary<Type, List<PropertyInfo>>();
            Registrations = new ConcurrentDictionary<Type, ConcurrentList<Registration>>();

            foreach (Type t in TypeFactory.GetAllTypes())
            {
                try
                {
                    if (t.IsAbstract || t.IsInterface)
                    { continue; }

                    ServiceProviderAttribute autoReg = t.GetCustomAttribute<ServiceProviderAttribute>();

                    if (autoReg != null)
                    {
                        Register(t, t, null, autoReg.ServiceProvider);
                    }

                    if (t.ImplementsInterface<IRegisterDependencies>())
                    {
                        StaticLogger.Log($"DependencyInjector: Registering {nameof(IRegisterDependencies)} of type {t.FullName} from assembly {t.Assembly.FullName}", StaticLogger.LoggingLevel.Call);
                        (Activator.CreateInstance(t) as IRegisterDependencies).RegisterDependencies();
                    }

                    if (t.ImplementsInterface<ISelfRegistering>())
                    {
                        Engine.Register(t, t, typeof(TransientServiceProvider));
                    }

                    if (t.IsSubclassOf(typeof(StaticServiceProvider)))
                    {
                        StaticServiceProvider provider = Activator.CreateInstance(t) as StaticServiceProvider;
                        StaticProviders.Add(t, provider);
                    }
                }
                catch (Exception ex)
                {
                    StaticLogger.Log("Failed to load information for type: " + t.FullName, StaticLogger.LoggingLevel.Call);
                    StaticLogger.Log(ex.StackTrace, StaticLogger.LoggingLevel.Call);
                    StaticLogger.Log(ex.Message, StaticLogger.LoggingLevel.Call);
                }
            }
        }

        /// <summary>
        /// Creates an instance of the engine and copies static providers to the instance provider pool
        /// </summary>
        public Engine()
        {
            if (!SingletonServiceProvider.Instances.ContainsKey(typeof(IServiceProvider)))
            {
                SingletonServiceProvider.Instances.TryAdd(typeof(IServiceProvider), new List<object> { this });
            }

            foreach (KeyValuePair<Type, AbstractServiceProvider> provider in StaticProviders)
            {
                AllProviders.Add(provider);
            }
        }

        /// <summary>
        /// Creates a clone of the current registrations and returns it.
        /// </summary>
        /// <returns>A clone of the current registrations</returns>
        public static ConcurrentDictionary<Type, ConcurrentList<Registration>> GetRegistrations()
        {
            ConcurrentDictionary<Type, ConcurrentList<Registration>> toReturn = new ConcurrentDictionary<Type, ConcurrentList<Registration>>();

            foreach (KeyValuePair<Type, ConcurrentList<Registration>> keyValuePair in Registrations)
            {
                ConcurrentList<Registration> list = new ConcurrentList<Registration>();

                foreach (Registration r in keyValuePair.Value)
                {
                    list.Add(r);
                }

                toReturn.TryAdd(keyValuePair.Key, list);
            }

            return toReturn;
        }

        /// <summary>
        /// Checks to see if a type is registered as an injection target
        /// </summary>
        /// <param name="t">The type to check for</param>
        /// <returns>Whether or not the type is registered as an injection target</returns>
        public static bool IsRegistered(Type t) => Registrations.ContainsKey(t);

        /// <summary>
        /// Try-Gets a list of registrations from the registration collection
        /// </summary>
        /// <param name="t">The type to check for</param>
        /// <param name="outT">If found, the return collection</param>
        /// <returns>Whether or not the type is registered as an injection target</returns>
        public static bool IsRegistered(Type t, out ConcurrentList<Registration> outT) => Registrations.TryGetValue(t, out outT);

        /// <summary>
        /// Resolves child properties of an object through the engine
        /// </summary>
        /// <typeparam name="T">Any class</typeparam>
        /// <param name="o">The object to resolve the properties of</param>
        /// <param name="resolutionPackage">Information to be used when resolving types</param>
        /// <returns>The passed in object with resolved properties (just in case)</returns>
        public static T ResolveProperties<T>(T o, ResolutionPackage resolutionPackage)
        {
            Type oType = o.GetType();

            if (!ChildDependancies.ContainsKey(oType))
            {
                ChildDependancies.TryAdd(oType, oType.GetProperties().Where(p => Attribute.IsDefined(p, typeof(DependencyAttribute))).ToList());
            }

            foreach (PropertyInfo thisDependency in ChildDependancies[oType])
            {
                if (!AnyRegistration(thisDependency.PropertyType))
                {
                    Type defaultType = thisDependency.GetCustomAttribute<DependencyAttribute>().Default;

                    thisDependency.SetValue(o, Resolve(
                        new Registration()
                        {
                            ServiceProvider = typeof(TransientServiceProvider),
                            RegisteredType = thisDependency.PropertyType,
                            ToInstantiate = defaultType ?? thisDependency.PropertyType
                        }, resolutionPackage).SingleOrDefault());
                }
                else
                {
                    thisDependency.SetValue(o, Resolve(thisDependency.PropertyType, resolutionPackage, true));
                }
            }

            return o;
        }

        /// <summary>
        /// Registers a service provider instance to be used for object resolution
        /// </summary>
        /// <param name="serviceProvider">The service provider to register</param>
        public void Register(AbstractServiceProvider serviceProvider)
        {
            if (serviceProvider is StaticServiceProvider)
            {
                StaticProviders.Add(serviceProvider.GetType(), serviceProvider as StaticServiceProvider);
            }
            else if (serviceProvider is ScopedServiceProvider)
            {
                ScopedProviders.Add(serviceProvider.GetType(), serviceProvider as ScopedServiceProvider);
            }
            else
            {
                throw new Exception("Service provider must inherit from either abstract or scoped");
            }

            AllProviders.Add(serviceProvider.GetType(), serviceProvider);
        }

        internal static ConcurrentDictionary<Type, List<PropertyInfo>> ChildDependancies { get; set; }
        internal static ConcurrentDictionary<Type, ConcurrentList<Registration>> Registrations { get; set; }
        internal static IDictionary<Type, AbstractServiceProvider> StaticProviders { get; set; } = new ConcurrentDictionary<Type, AbstractServiceProvider>();
        internal IDictionary<Type, AbstractServiceProvider> AllProviders { get; set; } = new ConcurrentDictionary<Type, AbstractServiceProvider>();
        internal IDictionary<Type, AbstractServiceProvider> ScopedProviders { get; set; } = new Dictionary<Type, AbstractServiceProvider>();

        internal static bool AnyRegistration(Type t)
        {
            return ResolveType(t).Any();
        }

        internal static object CreateRegisteredInstance(Registration registration, ResolutionPackage resolutionPackage, bool optional = false)
        {
            object toReturn = null;

            if (registration.InjectionFactory != null)
            {
                toReturn = registration.InjectionFactory.Invoke(Resolve<IServiceProvider>(resolutionPackage) ?? new Engine());
            }
            else if (registration.ToInstantiate != null && (!registration.ToInstantiate.IsInterface && !registration.ToInstantiate.IsAbstract))
            {
                toReturn = InstantiateObject(registration, resolutionPackage, optional);
            }

            if (toReturn != null)
            {
                ResolveProperties(toReturn, resolutionPackage);
            }

            return toReturn;
        }

        /// <summary>
        /// Creates a temporary registration instance to use in building resolution lists dynamically
        /// </summary>
        /// <param name="y">If requested</param>
        /// <param name="x">The Created</param>
        /// <param name="injectionFactory">using method</param>
        /// <param name="serviceProvider">With this scope</param>
        /// <returns></returns>
        internal static Registration GenerateRegistration(Type y, Type x, Func<IServiceProvider, object> injectionFactory = null, Type serviceProvider = null)
        {
            serviceProvider = serviceProvider ?? typeof(TransientServiceProvider);

            Registration thisRegistration = new Registration()
            {
                ServiceProvider = serviceProvider,
                RegisteredType = y,
                ToInstantiate = x,
                InjectionFactory = injectionFactory
            };

            if (!thisRegistration.ServiceProvider.IsSubclassOf(typeof(AbstractServiceProvider)))
            {
                throw new Exception($"Attemting to add a service provider registration for non service provider type {serviceProvider.Name}");
            }

            return thisRegistration;
        }

        internal static object InstantiateObject(Registration registration, ResolutionPackage resolutionPackage, bool optional = false)
        {
            ConstructorInfo[] Constructors = registration.ToInstantiate.GetConstructors();

            foreach (ConstructorInfo thisConstructor in Constructors.OrderByDescending(c => c.GetParameters().Count()))
            {
                ParameterInfo[] Parameters = thisConstructor.GetParameters();

                bool IsMatch = true;

                foreach (ParameterInfo thisParameter in Parameters)
                {
                    if (!IsResolvable(thisParameter.ParameterType) && !thisParameter.IsOptional)
                    {
                        IsMatch = false;
                        break;
                    }
                }

                if (IsMatch)
                {
                    object[] Params = Parameters.Select(p => Resolve(p.ParameterType, resolutionPackage)).ToArray();
                    return Activator.CreateInstance(registration.ToInstantiate, Params);
                }
            }

            if (!optional)
            {
                StringBuilder registered = new StringBuilder();

                foreach (KeyValuePair<Type, ConcurrentList<Registration>> r in Engine.Registrations)
                {
                    registered.Append(r.Key.Name + System.Environment.NewLine);

                    foreach (Registration thisRegistration in r.Value)
                    {
                        registered.Append($"\t{thisRegistration.RegisteredType.FullName} => {thisRegistration.ToInstantiate.FullName} as {thisRegistration.ServiceProvider.FullName}");
                    }
                }

                MissingInjectableConstructorException exception = new MissingInjectableConstructorException(registration.ToInstantiate);

                string DebugText = registered.ToString();

                string Error = $"Type ({registration.ToInstantiate.FullName}) does not contain constructor with registered parameters...";

                foreach (ConstructorInfo constructor in Constructors)
                {
                    FailingConstructor failingConstructor = new FailingConstructor
                    {
                        Constructor = constructor,
                        MissingParameters = constructor.GetParameters().Where(t => !IsRegistered(t.ParameterType)).ToArray()
                    };

                    Error += System.Environment.NewLine + string.Join(", ", constructor.GetParameters().Select(s => $"{s.ParameterType.FullName} {s.Name}"));
                    Error += System.Environment.NewLine + "Missing registrations: " + string.Join(", ", failingConstructor.MissingParameters.Select(s => $"{s.ParameterType.FullName} {s.Name}"));
                }

                exception.SetMessage(Error);

                throw exception;
            }
            else
            {
                return null;
            }
        }


        private static ConcurrentDictionary<Type, bool> ResolvableTypes { get; set; } = new ConcurrentDictionary<Type, bool>();
        internal static bool IsResolvable(Type t)
        {
            if (ResolvableTypes.TryGetValue(t, out bool toReturn))
            {
                return true;
            }

            bool alreadyResolvable = IsValidIEnumerable(t) || ResolveType(t).Any();

            if (!alreadyResolvable)
            {
                if (typeof(ISelfRegistering).IsAssignableFrom(t))
                {
                    if (typeof(IRegisterMostDerived).IsAssignableFrom(t))
                    {
                        Type toRegister = TypeFactory.GetMostDerivedType(t);

                        RegisterAllBaseTypes(t, toRegister, typeof(TransientServiceProvider));
                    }
                    else
                    {
                        Register(t, t, typeof(TransientServiceProvider));
                    }
                    toReturn = true;
                }
            }
            else
            {
                toReturn = true;
            }

            ResolvableTypes.TryAdd(t, toReturn);

            return toReturn;
        }

        internal static bool IsValidIEnumerable(Type t)
        {
            return (typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string));
        }

        internal static IEnumerable<Registration> ResolveType(Type t)
        {
            //Switch this to use CoreType and GetCollectionType so it handles arrays
            bool digDeeper = true;
            if (IsRegistered(t, out ConcurrentList<Registration> match))
            {
                foreach(Registration r in match)
                {
                    yield return r;
                    digDeeper = false;
                }
            }


            if (t.IsGenericType)
            {
                Type genericTypeDefinition = t.GetGenericTypeDefinition();

                if (IsRegistered(genericTypeDefinition, out ConcurrentList<Registration> genericMatchList))
                {

                    foreach (Registration genericMatch in genericMatchList)
                    {
                        if (genericMatch.ToInstantiate.IsGenericType)
                        {
                            Type newInstantiation = genericMatch.ToInstantiate.MakeGenericType(t.GenericTypeArguments);

                            yield return GenerateRegistration(t, newInstantiation);
                            digDeeper = false;
                        }
                    }
                }
            }

            if (digDeeper && IsValidIEnumerable(t))
            {
                foreach(Registration r in ResolveType(t.GetCollectionType())) {
                    yield return r;
                }
            }
        }
    }
}