using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSiteMapProvider.Core
{
    internal class InnerDependencyResolver
        : IDependencyResolver
    {
        private static readonly IDependencyResolver InnerResolver = new InnerDependencyResolver();

        public static IDependencyResolver Instance
        {
            get { return InnerResolver; }
        }

        private Dictionary<Type, object> dependencies = new Dictionary<Type, object>();

        private InnerDependencyResolver() 
        {
            dependencies.Add(typeof(ISiteMapSource), new SiteMapXmlSiteMapSource());
            dependencies.Add(typeof(IRequestContextProvider), new MvcHandlerRequestContextProvider());
            dependencies.Add(typeof(INodeKeyGenerator), new DefaultNodeKeyGenerator());
            dependencies.Add(typeof(XSiteMapTreeOptimizer), new XSiteMapTreeOptimizer());
            // TODO dependencies
            dependencies.Add(typeof(ISiteMapNodeUrlResolver), new DefaultSiteMapNodeUrlResolver());
            dependencies.Add(typeof(ISiteMapNodeVisibilityProvider), null);
        }

        public object GetService(Type serviceType)
        {
            return GetServices(serviceType).FirstOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            object service = null;
            if (dependencies.TryGetValue(serviceType, out service))
            {
                return new [] { service };
            }
            return new object[0];
        }
    }
}