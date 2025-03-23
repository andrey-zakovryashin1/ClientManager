namespace ClientManager.Models
{
    // <summary>
    /// Represents a view model for pagination including the current page number and total pages.
    /// </summary>
    public class PageViewModel
    {
        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageViewModel"/> class.
        /// </summary>
        /// <param name="count">The total number of items.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
