using System.Collections.Generic;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// IDynamicNodeProvider interface.
    /// </summary>
    public interface IDynamicNodeProvider
    {
        /// <summary>
        /// Gets the dynamic node collection.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <returns>
        /// A dynamic node collection.
        /// </returns>
        IEnumerable<XSiteMapNode> GetDynamicNodeCollection(XSiteMapNode currentNode);

        ///// <summary>
        ///// Gets a cache description for the dynamic node collection 
        ///// or null if there is none.
        ///// </summary>
        ///// <returns>
        ///// A cache description represented as a <see cref="CacheDescription"/> instance .
        ///// </returns>
        //CacheDescription GetCacheDescription();
    }
}