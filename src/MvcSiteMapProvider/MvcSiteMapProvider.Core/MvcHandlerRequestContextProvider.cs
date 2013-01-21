using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// MvcHandlerRequestContextProviderclass.
    /// </summary>
    public class MvcHandlerRequestContextProvider
        : IRequestContextProvider
    {
        /// <summary>
        /// Gets the current RequestContext.
        /// </summary>
        /// <returns></returns>
        public RequestContext GetRequestContext()
        {
            if (HttpContext.Current.Handler is MvcHandler)
            {
                return ((MvcHandler) HttpContext.Current.Handler).RequestContext;
            }
            return new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData());
        }
    }
}