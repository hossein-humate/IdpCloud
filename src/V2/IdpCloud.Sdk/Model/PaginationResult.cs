using System.Collections.Generic;

namespace IdpCloud.Sdk.Model
{
    /// <summary>
    /// The generic pagination result of a resource
    /// </summary>
    /// <typeparam name="TResource">The resource type class</typeparam>
    public abstract class PaginationResult<TResource>
      where TResource : class
    {
        /// <summary>
        /// Gets or sets the items in this page of the collection
        /// </summary>
        public IEnumerable<TResource> Items { get; set; }

        /// <summary>
        /// Gets or sets the total number of items in the collections
        /// </summary>
        public int TotalItems { get; set; }
    }
}
