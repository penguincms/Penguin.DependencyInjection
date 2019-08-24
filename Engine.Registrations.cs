using Penguin.Debugging;
using Penguin.DependencyInjection.Objects;
using Penguin.DependencyInjection.ServiceProviders;
using System;

namespace Penguin.DependencyInjection
{
    public partial class Engine
    {
        #region Methods

        /// <summary>
        /// Checks if theres an existing registration for this type
        /// </summary>
        /// <typeparam name="T">The type to check for</typeparam>
        /// <returns> if theres an existing registration for this type</returns>
        public static bool IsRegistered<T>() => Engine.IsRegistered(typeof(T));

        /// <summary>
        /// Creates a permanent type mapping for the registration and adds it to the cache
        /// </summary>
        /// <param name="y">If requested</param>
        /// <param name="x">The Created</param>
        /// <param name="injectionFactory">using method</param>
        /// <param name="lifetimeManager">With this scope</param>
        public static void Register(Type y, Type x, Func<IServiceProvider, object> injectionFactory = null, Type lifetimeManager = null) => AddRegistration(GenerateRegistration(y, x, injectionFactory, lifetimeManager));

        /// <summary>
        /// Registers a type using the specified lifetime manager
        /// </summary>
        /// <typeparam name="Y">The type to register</typeparam>
        /// <typeparam name="X">The type to return</typeparam>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public static void Register<Y, X>(Type lifetimeManager = null) where X : Y => Engine.Register(typeof(Y), typeof(X), lifetimeManager);

        /// <summary>
        /// Registers a type using the specified lifetime manager
        /// </summary>
        /// <param name="y">The type to register</param>
        /// <param name="x">The type to return</param>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public static void Register(Type y, Type x, Type lifetimeManager = null) => Engine.Register(y, x, null, lifetimeManager);

        /// <summary>
        /// Registers a type with a func to provide an instance later
        /// </summary>
        /// <typeparam name="Y">The type to register</typeparam>
        /// <param name="injectionFactory">The func to create an instance of the object</param>
        /// <param name="lifetimeManager">The type of ServiceProvider that should store the creaete instance</param>
        public static void Register<Y>(Func<IServiceProvider, object> injectionFactory, Type lifetimeManager = null) => Engine.Register(typeof(Y), injectionFactory, lifetimeManager);

        /// <summary>
        /// Registers a type with a func to provide an instance later
        /// </summary>
        /// <param name="y">The type to register</param>
        /// <param name="injectionFactory">The func to create an instance of the object</param>
        /// <param name="lifetimeManager">The type of ServiceProvider that should store the creaete instance</param>
        public static void Register(Type y, Func<IServiceProvider, object> injectionFactory, Type lifetimeManager = null) => Engine.Register(y, null, injectionFactory, lifetimeManager);

        /// <summary>
        /// Registers all types between the two given types (in a heiararchy) to resolve to the first type (inclusive)
        /// </summary>
        /// <param name="Base">The most derived type in the stack</param>
        /// <param name="Parent">The least derived type in the stack</param>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public static void RegisterAllBaseTypes(Type Base, Type Parent, Type lifetimeManager = null)
        {
            Type BaseType = Parent;
            do
            {
                Register(BaseType, Parent, null, lifetimeManager);
                BaseType = BaseType.BaseType;
            } while (BaseType != Base);

            Register(Base, Parent, null, lifetimeManager);
        }

        /// <summary>
        /// Registers all types between the two given types (in a heiararchy) to resolve to the first type (inclusive)
        /// </summary>
        /// <typeparam name="Base">The most derived type in the stack</typeparam>
        /// <typeparam name="Parent">The least derived type in the stack</typeparam>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public static void RegisterAllBaseTypes<Base, Parent>(Type lifetimeManager = null) where Parent : Base => Engine.RegisterAllBaseTypes(typeof(Base), typeof(Parent), lifetimeManager);


        /// <summary>
        /// Registers a concrete object instance to the given provider
        /// </summary>
        /// <param name="y">The tyoe to register</param>
        /// <param name="o">The object instance to register</param>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public static void RegisterInstance(Type y, object o, Type lifetimeManager = null)
        {
            if (o is null)
            {
                throw new Exception("Can not register null instance");
            }
            else if (!y.IsAssignableFrom(o.GetType()))
            {
                throw new ArgumentException($"Can not register object of type {o.GetType().FullName} to type {y.FullName} because it is not assignable");
            }

            lifetimeManager = lifetimeManager ?? typeof(SingletonServiceProvider);

            Registration thisRegistration = new Registration()
            {
                ServiceProvider = lifetimeManager,
                RegisteredType = y
            };

            AbstractServiceProvider instance = Activator.CreateInstance(lifetimeManager) as AbstractServiceProvider;

            instance.Add(y, o);

            AddRegistration(thisRegistration);
        }

        /// <summary>
        /// Registers a concrete object instance to the given provider
        /// </summary>
        /// <typeparam name="Y">The tyoe to register</typeparam>
        /// <param name="o">The object instance to register</param>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public static void RegisterInstance<Y>(object o, Type lifetimeManager = null) => Engine.RegisterInstance(typeof(Y), o, lifetimeManager);

        /// <summary>
        /// Removes any registrations for the given type
        /// </summary>
        /// <param name="t">The type to remove</param>
        /// <returns>Whether or not the unregistration was a success</returns>
        public static bool Unregister(Type t) => Engine.Registrations.TryRemove(t, out SynchronizedCollection<Registration> _);

        /// <summary>
        /// Registers a concrete object instance to the given provider
        /// </summary>
        /// <typeparam name="T">The type to remove</typeparam>
        /// <returns>Whether or not the unregistration was a success</returns>
        public static bool Unregister<T>() => Engine.Registrations.TryRemove(typeof(T), out SynchronizedCollection<Registration> _);

        /// <summary>
        /// Registers all parent types of the given object to the specified instance
        /// </summary>
        /// <param name="Base">The base type to use as a cut off for the registrations (so we dont register something like "object)</param>
        /// <param name="Instance">The object instance to register</param>
        /// <param name="lifetimeManager">The type of the ServiceProvider to use for resolution</param>
        public void RegisterInstanceAllBaseTypes(Type Base, object Instance, Type lifetimeManager = null)
        {
            Type BaseType = Instance.GetType();
            do
            {
                RegisterInstance(BaseType, Instance, lifetimeManager);
                BaseType = BaseType.BaseType;
            } while (BaseType != Base);

            RegisterInstance(Base, Instance, lifetimeManager);
        }

        internal static void AddRegistration(Registration dr)
        {
            if (StaticLogger.Level != StaticLogger.LoggingLevel.None) {
                StaticLogger.Log($"DI: Registering {dr.RegisteredType.FullName} => { dr.ToInstantiate.FullName} ({dr.ServiceProvider.Name})", StaticLogger.LoggingLevel.Call);
            }

            if (!Registrations.ContainsKey(dr.RegisteredType))
            {
                Registrations.TryAdd(dr.RegisteredType, new SynchronizedCollection<Registration>());
            }

            Registrations[dr.RegisteredType].Add(dr);

            return;
        }

        #endregion Methods
    }
}