namespace ClientManager.Models
{
    /// <summary>
    /// Represents a view model for filtering clients based on a search term.
    /// </summary>
    public class FilterViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterViewModel"/> class.
        /// </summary>
        /// <param name="filterText">The search term used to filter clients.</param>
        public FilterViewModel(string filterText)
        {
            SelectedText = filterText;
        }

        /// <summary>
        /// Gets the search term used for filtering clients.
        /// </summary>
        public string SelectedText { get; }
    }
}
