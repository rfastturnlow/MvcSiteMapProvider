using System;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// MvcSiteMapException class.
    /// </summary>
    public class MvcSiteMapException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcSiteMapException" /> class.
        /// </summary>
        public MvcSiteMapException()
            : this("")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcSiteMapException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MvcSiteMapException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcSiteMapException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MvcSiteMapException(string message, Exception innerException) : base(message, innerException) { }
    }
}