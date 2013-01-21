using System;
using System.Collections.Generic;
using MvcSiteMapProvider.Core;
using MvcMusicStore.Models;

namespace MvcMusicStore.Code
{
    /// <summary>
    /// StoreDetailsDynamicNodeProvider class
    /// </summary>
    public class StoreDetailsDynamicNodeProvider
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
            // Create a cloned node for each album
            foreach (var album in storeDB.Albums.Include("Genre"))
            {
                var node = currentNode.Clone() as XMvcSiteMapNode;
                node.Title = album.Title;
                node.ParentNode = currentNode.FindClosestParent("Genre_" + album.Genre.Name);
                node.RouteValues.Add("id", album.AlbumId);

                if (album.Title.Contains("Hit") || album.Title.Contains("Best"))
                {
                    node.Attributes.Add("bling", "<span style=\"color: Red;\">hot!</span>");
                }

                yield return node;
            }
        }

        ///// <summary>
        ///// Gets a cache description for the dynamic node collection 
        ///// or null if there is none.
        ///// </summary>
        ///// <returns>
        ///// A cache description represented as a <see cref="CacheDescription"/> instance .
        ///// </returns>
        //public override CacheDescription GetCacheDescription()
        //{
        //    return new CacheDescription("StoreDetailsDynamicNodeProvider")
        //    {
        //        SlidingExpiration = TimeSpan.FromMinutes(1)
        //    };
        //}
    }
}