namespace ClientManager.Models
{
    /// <summary>
    /// Represents a view model for the Index page, containing data for clients, pagination, filtering and sorting.
    /// </summary>
    public class IndexViewModel
    {
        // <summary>
        /// Gets the list of clients to display.
        /// </summary>
        public IEnumerable<Client> Clients { get; }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        public PageViewModel PageViewModel { get; }

        /// <summary>
        /// Gets the filtering data.
        /// </summary>
        public FilterViewModel FilterViewModel { get; }

        /// <summary>
        /// Gets the sorting data.
        /// </summary>
        public SortViewModel SortViewModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        /// <param name="clients">The list of clients to display.</param>
        /// <param name="pageViewModel">The pagination data.</param>
        /// <param name="filterViewModel">The filtering data.</param>
        /// <param name="sortViewModel">The sorting data.</param>
        public IndexViewModel(IEnumerable<Client> clients, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Clients = clients;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}
