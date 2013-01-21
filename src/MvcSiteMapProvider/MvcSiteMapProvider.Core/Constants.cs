namespace MvcSiteMapProvider.Core
{
    /// <summary>
    /// Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Cache keys.
        /// </summary>
        internal static class Cache
        {
            public static string PerRequestRootNodePrefix = "__mvcsitemapprovider_rootnodeperrequest_";
            public static string BaseTreePrefix = "__mvcsitemapprovider_basetree_";
        }
    }
}