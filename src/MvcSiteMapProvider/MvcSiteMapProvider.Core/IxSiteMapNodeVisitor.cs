namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// IxSiteMapNodeVisitor interface.
    /// </summary>
    public interface IxSiteMapNodeVisitor
    {
        /// <summary>
        /// Visits the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Visit(XSiteMapNode node);
    }
}