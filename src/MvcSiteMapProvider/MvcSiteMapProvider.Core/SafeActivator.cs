using System;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// SafeActivator class.
    /// </summary>
    internal static class SafeActivator
    {
        /// <summary>
        /// Creates an instance of the type designated by the specified generic type parameter, using the parameterless constructor .
        /// </summary>
        /// 
        /// <returns>
        /// A reference to the newly created object.
        /// </returns>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <exception cref="T:System.MissingMethodException">The type that is specified for <paramref name="T"/> does not have a parameterless constructor.</exception>
        public static object CreateInstance(Type type)
        {
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }
    }
}