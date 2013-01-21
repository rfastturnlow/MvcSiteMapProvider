using System.Security.Principal;
using System.Web;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// xSiteMapNodeVisitorBase class.
    /// </summary>
    public abstract class xSiteMapNodeVisitorBase
        : IxSiteMapNodeVisitor
    {
        /// <summary>
        /// Visits the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Visit(XSiteMapNode node)
        {
            VisitNode(node);
            if (node is XRouteSiteMapNode) VisitNode(node as XRouteSiteMapNode);
            if (node is XMvcSiteMapNode) VisitNode(node as XMvcSiteMapNode);

            foreach (var childNode in node.ChildNodes)
            {
                Visit(childNode);
            }
        }

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void VisitNode(XSiteMapNode node)
        {
        }

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void VisitNode(XRouteSiteMapNode node)
        {
        }

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void VisitNode(XMvcSiteMapNode node)
        {
        }
    }
}