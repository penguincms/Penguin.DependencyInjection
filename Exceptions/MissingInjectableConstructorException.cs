using System;
using System.Collections.Generic;
using System.Reflection;

namespace Penguin.DependencyInjection.Exceptions
{
    /// <summary>
    /// Contains information about an individual constructor that was not able to be injected, and why
    /// </summary>
    public class FailingConstructor
    {
        /// <summary>
        /// The constructor info for this constructor instance
        /// </summary>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// The constructor parameters that were not found registered in the dependency injector
        /// </summary>
        public IEnumerable<ParameterInfo> MissingParameters { get; set; }
    }

    /// <summary>
    /// Exception that contains information needed to understand why an object was not able to be constructed
    /// </summary>
    public class MissingInjectableConstructorException : Exception
    {
        /// <summary>
        /// Information for each constructor that was tried, and what it was missing
        /// </summary>
        public List<FailingConstructor> FailedConstructors { get; }

        /// <summary>
        /// The type that was attempted to be constructed unsuccessfully
        /// </summary>
        public Type FailingType { get; set; }

        /// <summary>
        /// A string message containing all of the information found in concrete form in this exception (for logging)
        /// </summary>
        public override string Message => this.MessageText;

        internal string MessageText { get; set; }

        internal MissingInjectableConstructorException(Type failingType)
        {
            this.FailingType = failingType;
            this.FailedConstructors = new List<FailingConstructor>();
        }

        internal void SetMessage(string message)
        {
            this.MessageText = message;
        }
    }
}