namespace ClientManager.Models
{
    /// <summary>
    /// Represents a view model for sorting client data.
    /// </summary>
    public class SortViewModel
    {
        /// <summary>
        /// Gets the sorting state for the first name column.
        /// </summary>
        public SortState FirstNameSort { get; }

        /// <summary>
        /// Gets the sorting state for the last name column.
        /// </summary>
        public SortState LastNameSort { get; }

        /// <summary>
        /// Gets the sorting state for the email column.
        /// </summary>
        public SortState EmailSort { get; }

        /// <summary>
        /// Gets the sorting state for the phone column.
        /// </summary>
        public SortState PhoneSort { get; }

        /// <summary>
        /// Gets the sorting state for the address column.
        /// </summary>
        public SortState AddressSort { get; }

        /// <summary>
        /// Gets the sorting state for the description column.
        /// </summary>
        public SortState DescriptionSort { get; }

        /// <summary>
        /// Gets the current sorting state.
        /// </summary>
        public SortState Current { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortViewModel"/> class.
        /// </summary>
        /// <param name="sortOrder">The current sorting order.</param>
        public SortViewModel(SortState sortOrder)
        {
            FirstNameSort = sortOrder == SortState.FirstNameAsc ? SortState.FirstNameDesc : SortState.FirstNameAsc;
            LastNameSort = sortOrder == SortState.LastNameAsc ? SortState.LastNameDesc : SortState.LastNameAsc;
            EmailSort = sortOrder == SortState.EmailAsc ? SortState.EmailDesc : SortState.EmailAsc;
            PhoneSort = sortOrder == SortState.PhoneAsc ? SortState.PhoneDesc : SortState.PhoneAsc;
            AddressSort = sortOrder == SortState.AddressAsc ? SortState.AddressDesc : SortState.AddressAsc;
            DescriptionSort = sortOrder == SortState.DescriptionAsc ? SortState.DescriptionDesc : SortState.DescriptionAsc; 
            Current = sortOrder;
        }
    }
}
