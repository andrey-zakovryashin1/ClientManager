namespace ClientManager.Models
{
    /// <summary>
    /// Represents the possible sorting states for client data.
    /// </summary>
    public enum SortState
    {
        /// <summary>
        /// Sort by first name in ascending order.
        /// </summary>
        FirstNameAsc,

        /// <summary>
        /// Sort by first name in descending order.
        /// </summary>
        FirstNameDesc,

        /// <summary>
        /// Sort by last name in ascending order.
        /// </summary>
        LastNameAsc,

        /// <summary>
        /// Sort by last name in descending order.
        /// </summary>
        LastNameDesc,

        /// <summary>
        /// Sort by email in ascending order.
        /// </summary>
        EmailAsc,

        /// <summary>
        /// Sort by email in descending order.
        /// </summary>
        EmailDesc,

        /// <summary>
        /// Sort by phone number in ascending order.
        /// </summary>
        PhoneAsc,

        /// <summary>
        /// Sort by phone number in descending order.
        /// </summary>
        PhoneDesc,

        /// <summary>
        /// Sort by address in ascending order.
        /// </summary>
        AddressAsc,

        /// <summary>
        /// Sort by address in descending order.
        /// </summary>
        AddressDesc,

        /// <summary>
        /// Sort by description in ascending order.
        /// </summary>
        DescriptionAsc,

        /// <summary>
        /// Sort by description in descending order.
        /// </summary>
        DescriptionDesc
    }
}