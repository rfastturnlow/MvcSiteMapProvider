using System;
using System.Collections.Generic;

namespace MvcSiteMapProvider.Core
{
    public class XRouteSiteMapNode
        : XSiteMapNode, ICloneable
    {        
        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        /// <value>The route.</value>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets the route values.
        /// </summary>
        /// <value>The route values.</value>
        public IDictionary<string, object> RouteValues { get; set; }

        /// <summary>
        /// Gets or sets the preserved route parameter names (= values that will be used from the current request route).
        /// </summary>
        /// <value>The attributes.</value>
        public IList<string> PreservedRouteParameters { get; set; }

        /// <summary>
        /// Gets or sets the type of the URL resolver.
        /// </summary>
        /// <value>
        /// The type of the URL resolver.
        /// </value>
        public string UrlResolverType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XRouteSiteMapNode"/> class.
        /// </summary>
        public XRouteSiteMapNode()
        {
            ChildNodes = new List<XSiteMapNode>();
            RouteValues = new Dictionary<string, object>();
            PreservedRouteParameters = new List<string>();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new XRouteSiteMapNode();
            clone.Key = this.Key;
            clone.ParentNode = this.ParentNode;
            clone.ChildNodes = new List<XSiteMapNode>();
            foreach (var childNode in ChildNodes)
            {
                var childClone = childNode.Clone() as XSiteMapNode;
                childClone.ParentNode = clone;
                clone.ChildNodes.Add(childClone);
            }
            clone.Url = this.Url;
            clone.HttpMethod = this.HttpMethod;
            clone.Clickable = this.Clickable;
            clone.ResourceKey = this.ResourceKey;
            clone.Title = this.Title;
            clone.Description = this.Description;
            clone.TargetFrame = this.TargetFrame;
            clone.ImageUrl = this.ImageUrl;
            clone.Attributes = new Dictionary<string, string>(this.Attributes);
            clone.Roles = new List<string>(this.Roles);
            clone.LastModifiedDate = this.LastModifiedDate;
            clone.ChangeFrequency = this.ChangeFrequency;
            clone.UpdatePriority = this.UpdatePriority;
            clone.VisibilityProviderType = this.VisibilityProviderType;

            clone.Route = this.Route;
            clone.RouteValues = new Dictionary<string, object>(this.RouteValues);
            clone.PreservedRouteParameters = this.PreservedRouteParameters;
            clone.UrlResolverType = this.UrlResolverType;

            return clone;
        }
    }
}