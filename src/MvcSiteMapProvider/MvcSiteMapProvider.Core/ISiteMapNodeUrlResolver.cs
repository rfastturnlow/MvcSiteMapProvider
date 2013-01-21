using System.Collections.Generic;
using System.Web;
using FakeN.Web;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// SiteMapNode URL resolver interface.
    /// </summary>
    public interface ISiteMapNodeUrlResolver
    {
        /// <summary>
        /// Resolves the URL.
        /// </summary>
        /// <param name="mvcSiteMapNode">The MVC site map node.</param>
        /// <param name="area">The area.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The resolved URL.</returns>
        string ResolveUrl(MvcSiteMapNode mvcSiteMapNode, string area, string controller, string action, IDictionary<string, object> routeValues);

        /// <summary>
        /// Resolves the URL.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns></returns>
        string ResolveUrl(XSiteMapNode node, HttpContextBase httpContext);
    }
}
