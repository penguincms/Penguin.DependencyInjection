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
        #region Properties
        /// <summary>
        /// The constructor info for this constructor instance
        /// </summary>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// The constructor parameters that were not found registered in the dependency injector
        /// </summary>
        public ParameterInfo[] MissingParameters { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Exception that contains information needed to understand why an object was not able to be constructed
    /// </summary>
    public class MissingInjectableConstructorException : Exception
    {
        #region Properties
        /// <summary>
        /// Information for each constructor that was tried, and what it was missing
        /// </summary>
        public List<FailingConstructor> FailedConstructors { get; set; }

        /// <summary>
        /// The type that was attempted to be constructed unsuccessfully
        /// </summary>
        public Type FailingType { get; set; }

        /// <summary>
        /// A string message containing all of the information found in concrete form in this exception (for logging)
        /// </summary>
        public override string Message { get { return _message; } }

        #endregion Properties

        #region Constructors


        internal MissingInjectableConstructorException(Type failingType)
        {
            FailingType = failingType;
            FailedConstructors = new List<FailingConstructor>();
        }

        #endregion Constructors

        internal string _message { get; set; }

        internal void SetMessage(string message)
        {
            _message = message;
        }
    }
}