using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;

namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// SiteMapSourceBase class.
    /// </summary>
    public abstract class SiteMapSourceBase
    {
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        protected string ProviderName { get; set; }

        /// <summary>
        /// Determines whether the site map source has data for site map provider with the specified provider name.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <returns>
        ///   <c>true</c> if the site map source has data for site map provider with the specified provider name; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasDataForSiteMapProvider(string providerName)
        {
            return providerName == null || providerName.ToLowerInvariant() == ProviderName.ToLowerInvariant();
        }

        /// <summary>
        /// Provides the base data on which the context-aware provider can generate a full tree.
        /// </summary>
        /// <param name="rootNode">The root node (can be null).</param>
        /// <returns></returns>
        public abstract XSiteMapNode ProvideBaseData(XSiteMapNode rootNode);

        /// <summary>
        /// Handle resource attribute
        /// </summary>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="text">Text</param>
        /// <param name="collection">NameValueCollection to be used for localization</param>
        protected static void HandleResourceAttribute(string attributeName, ref string text, ref NameValueCollection collection)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string tempStr1;
                var tempStr2 = text.TrimStart(new[] { ' ' });
                if (((tempStr2.Length > 10)) && tempStr2.ToLower(CultureInfo.InvariantCulture).StartsWith("$resources:", StringComparison.Ordinal))
                {
                    tempStr1 = tempStr2.Substring(11);
                    string tempStr3;
                    string tempStr4;
                    var index = tempStr1.IndexOf(',');
                    tempStr3 = tempStr1.Substring(0, index);
                    tempStr4 = tempStr1.Substring(index + 1);
                    var length = tempStr4.IndexOf(',');
                    if (length != -1)
                    {
                        text = tempStr4.Substring(length + 1);
                        tempStr4 = tempStr4.Substring(0, length);
                    }
                    else
                    {
                        text = null;
                    }
                    if (collection == null)
                    {
                        collection = new NameValueCollection();
                    }
                    collection.Add(attributeName, tempStr3.Trim());
                    collection.Add(attributeName, tempStr4.Trim());
                }
            }
        }

        /// <summary>
        /// Determines whether the specified node has dynamic nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the specified node has dynamic nodes; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool HasDynamicNodes(XSiteMapNode node)
        {
            return node.Attributes.ContainsKey("dynamicNodeProvider") && !string.IsNullOrEmpty(node.Attributes["dynamicNodeProvider"]);
        }

        /// <summary>
        /// Build the dynamic nodes for a given child node.
        /// </summary>
        /// <param name="childNode">The child node.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        protected virtual IEnumerable<XSiteMapNode> BuildDynamicNodesFor(XSiteMapNode childNode, XSiteMapNode parentNode)
        {
            // List of dynamic nodes that have been created
            var createdDynamicNodes = new List<XSiteMapNode>();

            // Dynamic nodes available?
            if (!HasDynamicNodes(childNode))
            {
                return createdDynamicNodes;
            }

            // Dependencies
            var nodeKeyGenerator = DependencyResolver.Current.GetService<INodeKeyGenerator>() ?? InnerDependencyResolver.Instance.GetService<INodeKeyGenerator>();

            // Instantiate provider
            var dynamicNodeProviderType = Type.GetType(childNode.Attributes["dynamicNodeProvider"]);
            var dynamicNodeProvider = (DependencyResolver.Current.GetService(dynamicNodeProviderType)
                                       ?? SafeActivator.CreateInstance(dynamicNodeProviderType)
                                       ?? DependencyResolver.Current.GetService<IDynamicNodeProvider>()) as IDynamicNodeProvider;
            if (dynamicNodeProvider == null)
            {
                throw new InstanceCouldNotBeCreatedException(dynamicNodeProviderType);
            }

            // Remove the internal attribute
            childNode.Attributes.Remove("dynamicNodeProvider");

            // Build dynamic nodes
            var currentNode = childNode.AsDynamicNode() as XSiteMapNode;
            foreach (var dynamicNode in dynamicNodeProvider.GetDynamicNodeCollection(currentNode))
            {
                // Casts
                var dynamicNodeAsMvcNode = dynamicNode as XMvcSiteMapNode;

                // Assign key if not specified
                if (string.IsNullOrEmpty(dynamicNode.Key))
                {
                    dynamicNode.Key = nodeKeyGenerator.GenerateKey(parentNode == null ? "" : parentNode.Key, Guid.NewGuid().ToString(),
                                                                   dynamicNode.Url,
                                                                   dynamicNode.Title,
                                                                   dynamicNodeAsMvcNode != null ? dynamicNodeAsMvcNode.Area : "",
                                                                   dynamicNodeAsMvcNode != null ? dynamicNodeAsMvcNode.Controller : "",
                                                                   dynamicNodeAsMvcNode != null ? dynamicNodeAsMvcNode.Action : "",
                                                                   dynamicNode.HttpMethod,
                                                                   dynamicNode.Clickable);
                }

                // And done!
                createdDynamicNodes.Add(dynamicNode);
            }

            // Done!
            return createdDynamicNodes;
        }
    }
}