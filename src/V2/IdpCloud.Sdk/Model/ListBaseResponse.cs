using System.Collections.Generic;

namespace IdpCloud.Sdk.Model
{
    /// <summary>
    /// A wrapper class for list of objects returned by this API.
    /// </summary>
    /// <typeparam name="T">The type of model to wrap.</typeparam>
    public class ListBaseResponse<T> : BaseResponse where T : class
    {
        /// <summary>
        /// Gets or sets the total number of pages in this collections.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of items in this collections.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the resources in this page of the collection.
        /// </summary>
        public List<T> Resources { get; set; }
    }
}
