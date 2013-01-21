using System;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// UnknownSiteMapProviderException class.
    /// </summary>
    public class InvalidElementException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementException" /> class.
        /// </summary>
        public InvalidElementException()
            : this(Resources.Messages.InvalidSiteMapElement)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidElementException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidElementException(string message, Exception innerException) : base(message, innerException) { }
    }
}
