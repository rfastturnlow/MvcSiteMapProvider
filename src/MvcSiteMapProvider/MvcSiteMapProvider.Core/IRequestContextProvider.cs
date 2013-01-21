using System.Web.Routing;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// IRequestContextProvider interface.
    /// </summary>
    public interface IRequestContextProvider
    {
        /// <summary>
        /// Gets the current RequestContext.
        /// </summary>
        /// <returns></returns>
        RequestContext GetRequestContext();
    }
}