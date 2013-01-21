using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;
using MvcSiteMapProvider.Core;

namespace MvcMusicStore.Code
{
    /// <summary>
    /// StoreBrowseDynamicNodeProvider class
    /// </summary>
    public class StoreBrowseDynamicNodeProvider
        : DynamicNodeProviderBase
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        /// <summary>
        /// Gets the dynamic node collection.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <returns>
        /// A dynamic node collection.
        /// </returns>
        public override IEnumerable<XSiteMapNode> GetDynamicNodeCollection(XSiteMapNode currentNode)
        {
            // Create a cloned node for each genre
            foreach (var genre in storeDB.Genres)
            {
                var node = currentNode.Clone() as XMvcSiteMapNode;
                node.Key = "Genre_" + genre.Name;
                node.Title = genre.Name;
                node.RouteValues["genre"] = genre.Name;

                yield return node; 
            }
        }
    }
}