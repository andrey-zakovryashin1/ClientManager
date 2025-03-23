using ClientManager.Models;

namespace ClientManager.Services
{
    /// <summary>
    /// Provides services for managing clients, including retrieving, updating and deleting client data.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Retrieves a paginated and sorted list of clients optionally filtered by a search text.
        /// </summary>
        /// <param name="filterText">The search text used to filter clients.</param>
        /// <param name="sortOrder">The sorting criteria for the client list.</param>
        /// <param name="page">The current page number for pagination.</param>
        /// <param name="pageSize">The number of clients to display per page.</param>
        /// <returns>A list of clients matching the specified criteria.</returns>
        Task<IEnumerable<Client>> GetClientsAsync(string filterText, SortState sortOrder, int page, int pageSize);

        /// <summary>
        /// Retrieves the total number of clients matching the specified filter.
        /// </summary>
        /// <param name="filterText">The search text used to filter clients.</param>
        /// <returns>The total number of clients matching the filter.</returns>
        Task<int> GetClientsCountAsync(string filterText);

        // <summary>
        /// Retrieves a client by its ID.
        /// </summary>
        /// <param name="id">The ID of the client to retrieve.</param>
        /// <returns>The client with the specified ID or null if not found.</returns>
        Task<Client?> GetClientByIdAsync(int id);

        /// <summary>
        /// Deletes a client by its ID. If the client has an associated address, it is also deleted.
        /// </summary>
        /// <param name="id">The ID of the client to delete.</param>
        Task DeleteClientAsync(int id);

        /// <summary>
        /// Updates the details of an existing client including its address if provided.
        /// </summary>
        /// <param name="client">The client object containing updated details.</param>
        Task UpdateClientAsync(Client client);

        /// <summary>
        /// Updates the details of an existing address.
        /// </summary>
        /// <param name="address">The address object containing updated details.</param>
        Task UpdateAddressAsync(Address address);
    }
}