using System;
using System.Collections.Generic;

namespace MvcSiteMapProvider.Core
{
    public class XMvcSiteMapNode
        : XRouteSiteMapNode, ICloneable
    {
        /// <summary>
        /// Gets or sets the area (optional).
        /// </summary>
        /// <value>The area.</value>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the controller (optional).
        /// </summary>
        /// <value>The controller.</value>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action (optional).
        /// </summary>
        /// <value>The action.</value>
        public string Action { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new XMvcSiteMapNode();
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

            clone.Action = this.Action;
            clone.Area = this.Area;
            clone.Controller = this.Controller;

            return clone;
        }
    }
}