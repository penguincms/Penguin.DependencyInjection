using Penguin.DependencyInjection.Objects;

namespace Penguin.DependencyInjection
{
    public partial class Engine
    {
        /// <summary>
        /// Attempts to resolve a service using registrations
        /// </summary>
        /// <typeparam name="T">The type of class being requested</typeparam>
        /// <returns>The registered or constructed instance of the class, or null, or error</returns>
        public static T GetService<T>() where T : class
        {
            return Engine.Resolve(typeof(T), new ResolutionPackage(StaticProviders)) as T;
        }
    }
}