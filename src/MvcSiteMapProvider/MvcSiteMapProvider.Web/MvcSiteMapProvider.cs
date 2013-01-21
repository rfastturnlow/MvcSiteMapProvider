using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcSiteMapProvider.Core;
using MvcSiteMapProvider.Core.Reflection;

namespace MvcSiteMapProvider.Web
{
    /// <summary>
    /// MvcSiteMapProvider class
    /// </summary>
    /// <remarks>
    /// Parameters from configuration are optional.
    ///    siteMapFile - either relative path or blank to fallback to IoC-provided ISiteMapSource
    ///    securityTrimmingEnabled - true or false; blank defaults to false
    ///    enableLocalization - true or false; blank defaults to false
    ///    scanAssembliesForSiteMapNodes - true or false; scan assemblies for sitemap nodes
    ///    includeAssembliesForScan - comma separated list of assemblies to include in scan (whitelist)
    ///    excludeAssembliesForScan - comma separated list of assemblies to exclude from scan (blacklist)
    /// </remarks>
    public class MvcSiteMapProvider
        : StaticSiteMapProvider
    {
        private object syncLock = new object();

        /// <summary>
        /// Gets or sets the site map source.
        /// </summary>
        /// <value>
        /// The site map source.
        /// </value>
        public ISiteMapSource SiteMapSource { get; protected set; }

        /// <summary>
        /// Initializes our custom provider, gets the attributes that are set in the config
        /// that enable us to customise the behaviour of this provider.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        public override void Initialize(string name, NameValueCollection attributes)
        {
            // Name given?
            if (string.IsNullOrEmpty(name))
            {
                name = string.Format("MvcSiteMapProvider_{0}", Guid.NewGuid());
            }

            // Initialize base
            base.Initialize(name, attributes);

            // Get the siteMapFile from the passed in attributes.
            string siteMapFile = attributes["siteMapFile"];
            if (!string.IsNullOrEmpty(siteMapFile))
            {
                // Do not use assembly scanner
                if (attributes["scanAssembliesForSiteMapNodes"] != null
                    && attributes["scanAssembliesForSiteMapNodes"].ToLowerInvariant() == "false")
                {
                    SiteMapSource = new SiteMapXmlSiteMapSource(name, siteMapFile);
                }
                else
                {
                    SiteMapSource = new AggregateSiteMapSource(name)
                    {
                        SiteMapSources = new List<ISiteMapSource>
                            {
                                new SiteMapXmlSiteMapSource(name, siteMapFile),
                                new ReflectionSiteMapSource(name, attributes["includeAssembliesForScan"], attributes["excludeAssembliesForScan"])
                            }
                    };
                }
            }
            else
            {
                SiteMapSource = DependencyResolver.Current.GetServices<ISiteMapSource>()
                    .FirstOrDefault(s => s.HasDataForSiteMapProvider(name)) ??
                                InnerDependencyResolver.Instance.GetService<ISiteMapSource>();
            }

            // Enable Localization
            if (!string.IsNullOrEmpty(attributes["enableLocalization"]))
            {
                EnableLocalization = Boolean.Parse(attributes["enableLocalization"]);
            }
        }

        /// <summary>
        /// Gets the current HTTP context.
        /// </summary>
        /// <value>
        /// The current HTTP context.
        /// </value>
        protected HttpContextBase CurrentContext
        {
            get
            {
                return (DependencyResolver.Current.GetService<IRequestContextProvider>() 
                    ?? InnerDependencyResolver.Instance.GetService<IRequestContextProvider>())
                        .GetRequestContext()
                        .HttpContext;
            }
        }

        /// <summary>
        /// Gets or sets the base tree.
        /// </summary>
        /// <value>
        /// The base tree.
        /// </value>
        protected XSiteMapNode BaseTree { get; set; }

        /// <summary>
        /// Determines whether the SiteMap tree is initialized for current request.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the SiteMap tree is initialized for current request; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsInitializedForCurrentRequest()
        {
            return CurrentContext.Items[Constants.Cache.PerRequestRootNodePrefix + Name] != null;
        }

        /// <summary>
        /// Returns the current root, otherwise calls the BuildSiteMap method.
        /// </summary>
        /// <returns></returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            if (!IsInitializedForCurrentRequest())
            {
                CurrentContext.Items[Constants.Cache.PerRequestRootNodePrefix + Name] = BuildSiteMap();
            }
            return CurrentContext.Items[Constants.Cache.PerRequestRootNodePrefix + Name] as SiteMapNode;
        }

        /// <summary>
        /// Builds the sitemap, firstly reads in the XML file, and grabs the outer root element and 
        /// maps this to become our main out root SiteMap node.
        /// </summary>
        /// <returns>The root SiteMapNode.</returns>
        public override SiteMapNode BuildSiteMap()
        {
            // Return immediately if this method has been called before
            if (IsInitializedForCurrentRequest())
            {
                return RootNode;
            }

            // Fetch the base tree from SiteMapSource
            if (BaseTree == null)
            {
                lock (syncLock)
                {
                    if (BaseTree == null)
                    {
                        // Build base tree
                        BaseTree = SiteMapSource.ProvideBaseData(null);

                        // Optimize base tree
                        var optimizer = (DependencyResolver.Current.GetService<XSiteMapTreeOptimizer>() ?? InnerDependencyResolver.Instance.GetService<XSiteMapTreeOptimizer>());
                        optimizer.Optimize(BaseTree);
                    }
                }
            }

            // TODO: map nodes and AddNode()
            // TODO: implement enrichment based on context -> in GetChildNodes.... and so on

            return null;
        }

        /// <summary>
        /// Removes all elements in the collections of child and parent site map nodes that the <see cref="T:System.Web.StaticSiteMapProvider" /> tracks as part of its state.
        /// </summary>
        protected override void Clear()
        {
            // TODO: implement
            BaseTree = null;
            base.Clear();
        }
    }
}