using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// AggregateSiteMapSource class.
    /// </summary>
    public class AggregateSiteMapSource
        : ISiteMapSource
    {
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        protected string ProviderName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSiteMapSource" /> class.
        /// </summary>
        public AggregateSiteMapSource()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSiteMapSource" /> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public AggregateSiteMapSource(string providerName)
        {
            ProviderName = providerName;

            SiteMapSources = new List<ISiteMapSource>();
        }

        /// <summary>
        /// Gets or sets the site map sources.
        /// </summary>
        /// <value>
        /// The site map sources.
        /// </value>
        public List<ISiteMapSource> SiteMapSources { get; set; }

        /// <summary>
        /// Determines whether the site map source has data for site map provider with the specified provider name.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <returns>
        ///   <c>true</c> if the site map source has data for site map provider with the specified provider name; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDataForSiteMapProvider(string providerName)
        {
            return SiteMapSources.Any(s => s.HasDataForSiteMapProvider(providerName));
        }

        /// <summary>
        /// Provides the base data on which the context-aware provider can generate a full tree.
        /// </summary>
        /// <param name="rootNode">The root node (can be null).</param>
        /// <returns></returns>
        public XSiteMapNode ProvideBaseData(XSiteMapNode rootNode) 
        {
            foreach (var siteMapSource in SiteMapSources)
            {
                try
                {
                    rootNode = siteMapSource.ProvideBaseData(rootNode);
                }
                catch (FileNotFoundException)
                {
                    // Swallow intentionally to support v3 behaviour.
                }
            }
            return rootNode;
        }
    }
}