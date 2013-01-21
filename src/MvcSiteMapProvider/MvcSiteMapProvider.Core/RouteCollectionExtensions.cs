using System.Web.Routing;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// RouteCollectionExtensions class.
    /// </summary>
    public static class RouteCollectionExtensions
    {
        /// <summary>
        /// Finds the first matching route.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="requestContext">The request context.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static RouteBase FindMatchingRoute(this RouteCollection collection, RequestContext requestContext, RouteValueDictionary values)
        {
            // Go through all the configured routes and find the first one that returns a match 
            using (collection.GetReadLock())
            {
                foreach (RouteBase route in collection)
                {
                    VirtualPathData vpd = route.GetVirtualPath(requestContext, values);
                    if (vpd != null)
                    {
                        return route;
                    }
                }
            }

            return null;
        }
    }
}