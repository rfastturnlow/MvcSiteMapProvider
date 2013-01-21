using System;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// InstanceCouldNotBeCreatedException class.
    /// </summary>
    public class InstanceCouldNotBeCreatedException : Exception
    {
        /// <summary>
        /// Gets or sets the type of the instance.
        /// </summary>
        /// <value>
        /// The type of the instance.
        /// </value>
        public Type InstanceType { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceCouldNotBeCreatedException" /> class.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        public InstanceCouldNotBeCreatedException(Type instanceType)
            : this(instanceType, string.Format(Resources.Messages.InstanceCouldNotBeCreated, instanceType))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceCouldNotBeCreatedException" /> class.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="message">The message.</param>
        public InstanceCouldNotBeCreatedException(Type instanceType, string message)
            : base(message)
        {
            InstanceType = instanceType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceCouldNotBeCreatedException" /> class.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InstanceCouldNotBeCreatedException(Type instanceType, string message, Exception innerException)
            : base(message, innerException)
        {
            InstanceType = instanceType;
        }
    }
}