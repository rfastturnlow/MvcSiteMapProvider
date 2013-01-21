using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// XSiteMapNodeExtensions class.
    /// </summary>
    public static class XSiteMapNodeExtensions
    {
        /// <summary>
        /// Gets the URL resolver.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static ISiteMapNodeUrlResolver GetUrlResolver(this XRouteSiteMapNode node)
        {
            if (string.IsNullOrEmpty(node.UrlResolverType))
            {
                return null;
            }

            var urlResolverType = Type.GetType(node.UrlResolverType);
            ISiteMapNodeUrlResolver urlResolver = null;
            if (urlResolverType != null)
            {
                return (DependencyResolver.Current.GetService(urlResolverType)
                               ?? SafeActivator.CreateInstance(urlResolverType)
                               ?? DependencyResolver.Current.GetService<ISiteMapNodeUrlResolver>()) as ISiteMapNodeUrlResolver;
            }
            else
            {
                return InnerDependencyResolver.Instance.GetService<ISiteMapNodeUrlResolver>();
            }
        }
    }
}
