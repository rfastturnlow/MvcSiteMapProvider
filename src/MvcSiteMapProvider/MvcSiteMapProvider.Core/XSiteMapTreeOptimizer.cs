using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using FakeN.Web;

namespace MvcSiteMapProvider.Core
{
    // TODO: for sample blog post 
    /// <summary>
    ///// Visits the node.
    ///// </summary>
    ///// <param name="node">The node.</param>
    //public override void VisitNode(XRouteSiteMapNode node)
    //{
    //    var nodeAsMvcNode = node as XMvcSiteMapNode;
    //    if (nodeAsMvcNode == null && !node.PreservedRouteParameters.Any() && string.IsNullOrEmpty(node.Url))
    //    {
    //        var urlResolver = node.GetUrlResolver();
    //        if (urlResolver != null)
    //        {
    //            var httpContext = new FakeHttpContext(new FakeHttpRequest(new Uri("http://localhost"), (node.HttpMethod == "*" ? "GET" : node.HttpMethod)));
    //            node.Url = urlResolver.ResolveUrl(node, httpContext);
    //            node.UrlResolverType = "";
    //        }
    //    }
    //    base.VisitNode(node);
    //}

    /// <summary>
    /// xSiteMapTreeOptimizer class. This class is resolved when building the base tree and can be used to optimize the initial tree.
    /// </summary>
    public class XSiteMapTreeOptimizer
        : xSiteMapNodeVisitorBase
    {
        /// <summary>
        /// Optimizes the specified tree.
        /// </summary>
        /// <param name="root">The root.</param>
        public virtual void Optimize(XSiteMapNode root)
        {
            Visit(root);
        }
    }
}