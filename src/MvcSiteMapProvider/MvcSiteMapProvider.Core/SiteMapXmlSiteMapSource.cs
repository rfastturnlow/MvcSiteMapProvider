using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// SiteMapXmlSiteMapSource lass.
    /// </summary>
    public class SiteMapXmlSiteMapSource
        : SiteMapSourceBase, ISiteMapSource
    {
        protected readonly string xmlSourceRootElementName = "mvcSiteMap";
        protected readonly string xmlSourceChildElementName = "mvcSiteMapNode";
        protected readonly XNamespace xmlSourceNamespace = "http://mvcsitemap.codeplex.com/schemas/MvcSiteMap-File-4.0";

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        protected SiteMapProvider Provider
        {
            get
            {
                if (ProviderName == null)
                {
                    return SiteMap.Provider;
                }
                return SiteMap.Providers[ProviderName];
            }
        }

        /// <summary>
        /// Gets or sets the site map file relative.
        /// </summary>
        /// <value>
        /// The site map file relative.
        /// </value>
        public string SiteMapFileRelative { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapXmlSiteMapSource" /> class.
        /// </summary>
        public SiteMapXmlSiteMapSource()
            : this(null, "~/Mvc.sitemap")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapXmlSiteMapSource" /> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="siteMapFileRelative">The site map file relative path.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public SiteMapXmlSiteMapSource(string providerName, string siteMapFileRelative)
        {
            ProviderName = providerName;

            var siteMapFileAbsolute = HostingEnvironment.MapPath(siteMapFileRelative);
            if (!File.Exists(siteMapFileAbsolute))
            {
                throw new FileNotFoundException(string.Format(Resources.Messages.SiteMapFileNotFound, siteMapFileRelative), siteMapFileAbsolute);
            }

            SiteMapFileRelative = siteMapFileRelative;
        }

        /// <summary>
        /// Provides the base data on which the context-aware provider can generate a full tree.
        /// </summary>
        /// <param name="rootNode">The root node (can be null).</param>
        /// <returns></returns>
        public override XSiteMapNode ProvideBaseData(XSiteMapNode rootNode)
        {
            var siteMapFileAbsolute = HostingEnvironment.MapPath(SiteMapFileRelative);
            if (!File.Exists(siteMapFileAbsolute))
            {
                throw new FileNotFoundException(string.Format(Resources.Messages.SiteMapFileNotFound, SiteMapFileRelative), siteMapFileAbsolute);
            }

            // Load the XML document.
            XDocument siteMapXml = XDocument.Load(siteMapFileAbsolute);

            // If no namespace is present (or the wrong one is present), replace it
            foreach (var e in siteMapXml.Descendants())
            {
                if (string.IsNullOrEmpty(e.Name.Namespace.NamespaceName) || e.Name.Namespace != xmlSourceNamespace)
                {
                    e.Name = XName.Get(e.Name.LocalName, xmlSourceNamespace.ToString());
                }
            }

            // Get the root mvcSiteMapNode element, and map this to an MvcSiteMapNode
            var rootElement = siteMapXml.Element(xmlSourceNamespace + xmlSourceRootElementName).Element(xmlSourceNamespace + xmlSourceChildElementName);
            rootNode = GetSiteMapNodeFromXmlElement(rootElement, null);

            // Process our XML file, passing in the main root sitemap node and xml element.
            ProcessXmlNodes(rootElement, rootNode);

            // Done!
            return rootNode;
        }

        /// <summary>
        /// Maps an XElement from the XML file to an <see cref="XSiteMapNode"/>.
        /// </summary>
        /// <param name="node">The element to map.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>An MvcSiteMapNode which represents the XElement.</returns>
        protected virtual XSiteMapNode GetSiteMapNodeFromXmlElement(XElement node, XSiteMapNode parentNode)
        {
            // Dependencies
            var nodeKeyGenerator = DependencyResolver.Current.GetService<INodeKeyGenerator>() ?? InnerDependencyResolver.Instance.GetService<INodeKeyGenerator>();

            // Get area, controller and action from node declaration
            string area = node.GetAttributeValue("area");
            string controller = node.GetAttributeValue("controller");
            string action = node.GetAttributeValue("action");
            string route = node.GetAttributeValue("route");

            // Determine the node type
            XSiteMapNode siteMapNode = null;
            if (!string.IsNullOrEmpty(area) || !string.IsNullOrEmpty(controller) || !string.IsNullOrEmpty(action))
            {
                siteMapNode = new XMvcSiteMapNode();
            }
            else if (!string.IsNullOrEmpty(route))
            {
                siteMapNode = new XRouteSiteMapNode();
            }
            else
            {
                siteMapNode = new XSiteMapNode();
            }

            // Generate key for node
            string key = nodeKeyGenerator.GenerateKey(
                parentNode == null ? "" : parentNode.Key,
                node.GetAttributeValue("key"),
                node.GetAttributeValue("url"),
                node.GetAttributeValue("title"),
                area,
                controller,
                node.GetAttributeValue("action"),
                node.GetAttributeValueOrFallback("httpMethod", "*").ToUpperInvariant(),
                !(node.GetAttributeValue("clickable") == "false"));

            // Handle title and description globalization
            var explicitResourceKeys = new NameValueCollection();
            var title = node.GetAttributeValue("title");
            var description = node.GetAttributeValue("description") ?? title;
            HandleResourceAttribute("title", ref title, ref explicitResourceKeys);
            HandleResourceAttribute("description", ref description, ref explicitResourceKeys);

            // Handle implicit resources
            var implicitResourceKey = node.GetAttributeValue("resourceKey");
            if (!string.IsNullOrEmpty(implicitResourceKey))
            {
                title = null;
                description = null;
            }

            // Assign defaults
            siteMapNode.Key = key;
            siteMapNode.Title = title;
            siteMapNode.Description = description;
            siteMapNode.ResourceKey = implicitResourceKey;
            siteMapNode.Attributes = AcquireAttributesFrom(node);
            siteMapNode.Roles = node.GetAttributeValue("roles").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            siteMapNode.Clickable = bool.Parse(node.GetAttributeValueOrFallback("clickable", "true"));
            siteMapNode.VisibilityProviderType = node.GetAttributeValue("visibilityProvider");
            siteMapNode.ImageUrl= node.GetAttributeValue("imageUrl");
            siteMapNode.TargetFrame= node.GetAttributeValue("targetFrame");
            siteMapNode.HttpMethod = node.GetAttributeValueOrFallback("httpMethod", "*").ToUpperInvariant();

            if (!siteMapNode.Clickable)
            {
                siteMapNode.Url = "";
            }
            else
            {
                siteMapNode.Url = node.GetAttributeValue("url");
            }
            if (!string.IsNullOrEmpty(node.GetAttributeValue("changeFrequency")))
            {
                siteMapNode.ChangeFrequency = (ChangeFrequency)Enum.Parse(typeof(ChangeFrequency), node.GetAttributeValue("changeFrequency"));
            }
            else
            {
                siteMapNode.ChangeFrequency = ChangeFrequency.Undefined;
            }
            if (!string.IsNullOrEmpty(node.GetAttributeValue("updatePriority")))
            {
                siteMapNode.UpdatePriority = (UpdatePriority)Enum.Parse(typeof(UpdatePriority), node.GetAttributeValue("updatePriority"));
            }
            else
            {
                siteMapNode.UpdatePriority = UpdatePriority.Undefined;
            }           
            if (!string.IsNullOrEmpty(node.GetAttributeValue("lastModifiedDate")))
            {
                siteMapNode.LastModifiedDate = DateTime.Parse(node.GetAttributeValue("lastModifiedDate"));
            }
            else
            {
                siteMapNode.LastModifiedDate = DateTime.MinValue;
            }

            // Handle route details
            var routeNode = siteMapNode as XRouteSiteMapNode;
            if (routeNode != null)
            {
                // Assign to node
                routeNode.Route = node.GetAttributeValue("route");
                routeNode.RouteValues = AcquireRouteValuesFrom(node);
                routeNode.PreservedRouteParameters = node.GetAttributeValue("preservedRouteParameters").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                routeNode.Url = "";
                routeNode.UrlResolverType = node.GetAttributeValue("urlResolver");

                // Add inherited route values to sitemap node
                var parentRouteNode = parentNode as XRouteSiteMapNode;
                if (parentRouteNode != null)
                {
                    foreach (var inheritedRouteParameter in node.GetAttributeValue("inheritedRouteParameters").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var item = inheritedRouteParameter.Trim();
                        if (parentRouteNode.RouteValues.ContainsKey(item))
                        {
                            routeNode.RouteValues.Add(item, parentRouteNode.RouteValues[item]);
                        }
                    }
                }
            }

            // Handle MVC details
            var mvcNode = siteMapNode as XMvcSiteMapNode;
            if (mvcNode != null)
            {
                // MVC properties
                mvcNode.Area = area;
                mvcNode.Controller = controller;
                mvcNode.Action = action;

                // Inherit area and controller from parent
                var parentMvcNode = parentNode as XMvcSiteMapNode;
                if (parentMvcNode != null)
                {
                    if (string.IsNullOrEmpty(area))
                    {
                        mvcNode.Area = parentMvcNode.Area;
                    }
                    if (string.IsNullOrEmpty(controller))
                    {
                        mvcNode.Controller = parentMvcNode.Controller;
                    }
                }

                // Add defaults for area
                if (!mvcNode.RouteValues.ContainsKey("area"))
                {
                    mvcNode.RouteValues.Add("area", "");
                }
            }

            return siteMapNode;
        }

        /// <summary>
        /// Acquires the attributes from a given XElement.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected virtual IDictionary<string, string> AcquireAttributesFrom(XElement node)
        {
            var returnValue = new Dictionary<string, string>();
            foreach (XAttribute attribute in node.Attributes())
            {
                var attributeName = attribute.Name.ToString();
                var attributeValue = attribute.Value;

                if (IsRegularAttribute(attributeName))
                {
                    returnValue.Add(attributeName, attributeValue);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Acquires the route values from a given XElement.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected virtual IDictionary<string, object> AcquireRouteValuesFrom(XElement node)
        {
            var returnValue = new Dictionary<string, object>();
            foreach (XAttribute attribute in node.Attributes())
            {
                var attributeName = attribute.Name.ToString();
                var attributeValue = attribute.Value;

                if (IsRouteAttribute(attributeName))
                {
                    returnValue.Add(attributeName, attributeValue);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Determines whether the attribute is a regular attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>
        ///   <c>true</c> if the attribute is a regular attribute; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsRegularAttribute(string attributeName)
        {
            return attributeName != "title"
                   && attributeName != "description";
        }

        /// <summary>
        /// Determines whether the attribute is a route attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>
        ///   <c>true</c> if the attribute is a route attribute; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsRouteAttribute(string attributeName)
        {
            return attributeName != "title"
                   && attributeName != "description"
                   && attributeName != "resourceKey"
                   && attributeName != "key"
                   && attributeName != "roles"
                   && attributeName != "route"
                   && attributeName != "url"
                   && attributeName != "clickable"
                   && attributeName != "httpMethod"
                   && attributeName != "urlResolver"
                   && attributeName != "visibilityProvider"
                   && attributeName != "lastModifiedDate"
                   && attributeName != "changeFrequency"
                   && attributeName != "updatePriority"
                   && attributeName != "targetFrame"
                   && attributeName != "imageUrl"
                   && attributeName != "inheritedRouteParameters"
                   && attributeName != "preservedRouteParameters"
                   // TODO implement && !attributesToIgnore.Contains(attributeName)
                   && !attributeName.StartsWith("data-");
        }

        /// <summary>
        /// Recursively processes our XML document, parsing our siteMapNodes and dynamicNode(s).
        /// </summary>
        /// <param name="rootElement">The main root XML element.</param>
        /// <param name="rootNode">The main root sitemap node.</param>
        protected virtual void ProcessXmlNodes(XElement rootElement, XSiteMapNode rootNode)
        {
            // Loop through each element below the current root element.
            foreach (XElement node in rootElement.Elements())
            {
                XSiteMapNode childNode;
                if (node.Name == xmlSourceNamespace + xmlSourceChildElementName)
                {
                    childNode = GetSiteMapNodeFromXmlElement(node, rootNode);
                    XSiteMapNode parentNode = rootNode;
                    childNode.ParentNode = parentNode;

                    if (HasDynamicNodes(childNode))
                    {
                        var dynamicNodesForChildNode = BuildDynamicNodesFor(childNode, parentNode);
                        foreach (var dynamicNode in dynamicNodesForChildNode)
                        {
                            // Verify parent/child relation
                            if (dynamicNode.ParentNode == parentNode
                                && parentNode.ChildNodes.All(n => n != dynamicNode))
                            {
                                parentNode.ChildNodes.Add(dynamicNode);
                            }

                            // Process possible XML-based childs
                            ProcessXmlNodes(node, dynamicNode);
                        }
                    }
                    else
                    {
                        parentNode.ChildNodes.Add(childNode);
                    }
                }
                else
                {
                    // If the current node is not one of the known node types throw and exception
                    throw new InvalidElementException();
                }

                // Continue recursively processing the XML file.
                ProcessXmlNodes(node, childNode);
            }
        }
    }
}