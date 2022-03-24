namespace IdpCloud.Sdk.Model
{
    /// <summary>
    /// The pagination request of a resource
    /// </summary>
    public abstract class PaginationParam
    {
        /// <summary>
        /// The number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The current active page index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Represent the filter string value(filter= col eq 2)
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Represent the sort order string value(orderby=name desc,id
        /// </summary>
        public string OrderBy { get; set; }
    }
}
