using Microsoft.EntityFrameworkCore;
using ClientManager.Models;

namespace ClientManager.Services
{
    /// <summary>
    /// Provides services for managing clients, including retrieving, updating and deleting client data.
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly ClientsContext db;
        private readonly ClientQueryService clientQueryService;
        private readonly ILogger<ClientService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientService"/> class.
        /// </summary>
        /// <param name="db">The database context for accessing client data.</param>
        /// <param name="clientQueryService">The service for querying client data.</param>
        /// <param name="logger">The logger for recording errors and information.</param>
        public ClientService(ClientsContext db, ClientQueryService clientQueryService, ILogger<ClientService> logger)
        {
            this.db = db;
            this.clientQueryService = clientQueryService;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated and sorted list of clients optionally filtered by a search text.
        /// </summary>
        /// <param name="filterText">The search text used to filter clients.</param>
        /// <param name="sortOrder">The sorting criteria for the client list.</param>
        /// <param name="page">The current page number for pagination.</param>
        /// <param name="pageSize">The number of clients to display per page.</param>
        /// <returns>A list of clients matching the specified criteria.</returns>
        public async Task<IEnumerable<Client>> GetClientsAsync(string filterText, SortState sortOrder, int page, int pageSize)
        {
            try
            {
                return await clientQueryService.GetClientsAsync(filterText, sortOrder, page, pageSize);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving clients.");
                throw new ApplicationException("An error occurred while retrieving clients.", ex);
            }
        }

        /// <summary>
        /// Retrieves the total number of clients matching the specified filter.
        /// </summary>
        /// <param name="filterText">The search text used to filter clients.</param>
        /// <returns>The total number of clients matching the filter.</returns>
        public async Task<int> GetClientsCountAsync(string filterText)
        {
            try
            {
                return await clientQueryService.GetClientsCountAsync(filterText);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while counting clients.");
                throw new ApplicationException("An error occurred while counting clients.", ex);
            }
        }

        /// <summary>
        /// Retrieves a client by its ID.
        /// </summary>
        /// <param name="id">The ID of the client to retrieve.</param>
        /// <returns>The client with the specified ID or null if not found.</returns>
        public async Task<Client?> GetClientByIdAsync(int id)
        {
            try
            {
                return await db.Clients.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving a client by ID.");
                throw new ApplicationException("An error occurred while retrieving a client by ID.", ex);
            }
        }

        /// <summary>
        /// Deletes a client by its ID. If the client has an associated address, it is also deleted.
        /// </summary>
        /// <param name="id">The ID of the client to delete.</param>
        public async Task DeleteClientAsync(int id)
        {
            try
            {
                var client = await db.Clients.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
                if (client == null)
                {
                    throw new KeyNotFoundException("Client not found");
                }

                db.Clients.Remove(client);
                if (client.Address != null)
                {
                    db.Addresses.Remove(client.Address);
                }
                await db.SaveChangesAsync();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Client not found for deletion.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "An error occurred while deleting a client.");
                throw new ApplicationException("An error occurred while deleting a client.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while deleting a client.");
                throw new ApplicationException("An unexpected error occurred while deleting a client.", ex);
            }
        }

        /// <summary>
        /// Updates the details of an existing client including its address if provided.
        /// </summary>
        /// <param name="client">The client object containing updated details.</param>
        public async Task UpdateClientAsync(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            try
            {
                var existingClient = await db.Clients
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.Id == client.Id);

                if (existingClient == null)
                {
                    throw new KeyNotFoundException("Client not found");
                }

                existingClient.FirstName = client.FirstName;
                existingClient.LastName = client.LastName;
                existingClient.Email = client.Email;
                existingClient.Phone = client.Phone;
                existingClient.Description = client.Description;

                if (client.Address != null)
                {
                    if (existingClient.Address != null)
                    {
                        existingClient.Address.StreetAddress = client.Address.StreetAddress;
                        existingClient.Address.City = client.Address.City;
                        existingClient.Address.State = client.Address.State;
                        existingClient.Address.Zip = client.Address.Zip;
                    }
                    else
                    {
                        existingClient.Address = new Address
                        {
                            StreetAddress = client.Address.StreetAddress,
                            City = client.Address.City,
                            State = client.Address.State,
                            Zip = client.Address.Zip
                        };
                    }
                }

                db.Clients.Update(existingClient);
                await db.SaveChangesAsync();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Client not found for update.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "An error occurred while updating the client in the database.");
                throw new ApplicationException("An error occurred while updating the client in the database.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while updating a client.");
                throw new ApplicationException("An unexpected error occurred while updating a client.", ex);
            }
        }

        /// <summary>
        /// Updates the details of an existing address.
        /// </summary>
        /// <param name="address">The address object containing updated details.</param>
        public async Task UpdateAddressAsync(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            try
            {

                var existingAddress = await db.Addresses.FirstOrDefaultAsync(a => a.Id == address.Id);

                if (existingAddress == null)
                {
                    throw new KeyNotFoundException("Address not found");
                }

                existingAddress.StreetAddress = address.StreetAddress;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.Zip = address.Zip;

                db.Addresses.Update(existingAddress);
                await db.SaveChangesAsync();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Address not found for update.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "An error occurred while updating the address in the database.");
                throw new ApplicationException("An error occurred while updating the address in the database.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while updating an address.");
                throw new ApplicationException("An unexpected error occurred while updating an address.", ex);
            }
        }
    }
}