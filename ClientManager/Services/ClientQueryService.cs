using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using ClientManager.Models;

[assembly: InternalsVisibleTo("ClientManager.Tests")]

namespace ClientManager.Services
{
    /// <summary>
    /// Provides querying functionality for clients, including filtering, sorting and pagination.
    /// </summary>
    public class ClientQueryService
    {
        private readonly ClientsContext db;
        private readonly ILogger<ClientQueryService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQueryService"/> class.
        /// </summary>
        /// <param name="db">The database context for accessing client data.</param>
        /// <param name="logger">The logger for recording errors and information.</param>
        public ClientQueryService(ClientsContext db, ILogger<ClientQueryService> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated and sorted list of clients optionally filtered by a search term.
        /// </summary>
        /// <param name="filterText">The search term used to filter clients.</param>
        /// <param name="sortOrder">The sorting criteria for the client list.</param>
        /// <param name="page">The current page number for pagination.</param>
        /// <param name="pageSize">The number of clients to display per page.</param>
        /// <returns>A list of clients matching the specified criteria.</returns>
        public async Task<IEnumerable<Client>> GetClientsAsync(string filterText, SortState sortOrder, int page, int pageSize)
        {
            try
            {
                var query = db.Clients.Include(c => c.Address).AsQueryable();

                query = ApplyFilter(query, filterText);
                query = ApplySorting(query, sortOrder);
                query = ApplyPagination(query, page, pageSize);

                return await query.ToListAsync();
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
        /// <param name="filterText">The search term used to filter clients.</param>
        /// <returns>The total number of clients matching the filter.</returns>
        public async Task<int> GetClientsCountAsync(string filterText)
        {
            try
            {
                var query = db.Clients.Include(c => c.Address).AsQueryable();
                query = ApplyFilter(query, filterText);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while counting clients.");
                throw new ApplicationException("An error occurred while counting clients.", ex);
            }
        }

        /// <summary>
        /// Applies a filter to the client query based on the specified search term.
        /// </summary>
        /// <param name="query">The query to filter.</param>
        /// <param name="filterText">The search term used to filter clients.</param>
        /// <returns>The filtered query.</returns>
        internal IQueryable<Client> ApplyFilter(IQueryable<Client> query, string filterText)
        {
            try
            {
                if (!string.IsNullOrEmpty(filterText))
                {
                    query = query.Where(c =>
                        (c.FirstName != null && c.FirstName.Contains(filterText)) ||
                        (c.LastName != null && c.LastName.Contains(filterText)) ||
                        (c.Email != null && c.Email.Contains(filterText)) ||
                        (c.Phone != null && c.Phone.Contains(filterText)) ||
                        (c.Address != null &&
                            ((c.Address.StreetAddress != null && c.Address.StreetAddress.Contains(filterText)) ||
                             (c.Address.City != null && c.Address.City.Contains(filterText)) ||
                             (c.Address.State != null && c.Address.State.Contains(filterText)) ||
                             (c.Address.Zip != null && c.Address.Zip.Contains(filterText)))) ||
                        (c.Description != null && c.Description.Contains(filterText))
                    );
                }

                return query;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying the filter.");
                throw new ApplicationException("An error occurred while applying the filter.", ex);
            }
        }

        // <summary>
        /// Applies sorting to the client query based on the specified sorting criteria.
        /// </summary>
        /// <param name="query">The query to sort.</param>
        /// <param name="sortOrder">The sorting criteria.</param>
        /// <returns>The sorted query.</returns>
        internal IQueryable<Client> ApplySorting(IQueryable<Client> query, SortState sortOrder)
        {
            try
            {
                return sortOrder switch
                {
                    SortState.FirstNameDesc => query.OrderByDescending(c => c.FirstName),
                    SortState.LastNameAsc => query.OrderBy(c => c.LastName),
                    SortState.LastNameDesc => query.OrderByDescending(c => c.LastName),
                    SortState.EmailAsc => query.OrderBy(c => c.Email),
                    SortState.EmailDesc => query.OrderByDescending(c => c.Email),
                    SortState.PhoneAsc => query.OrderBy(c => c.Phone),
                    SortState.PhoneDesc => query.OrderByDescending(c => c.Phone),
                    SortState.AddressAsc => query.OrderBy(c => c.Address.StreetAddress),
                    SortState.AddressDesc => query.OrderByDescending(c => c.Address.StreetAddress),
                    SortState.DescriptionAsc => query.OrderBy(c => c.Description),
                    SortState.DescriptionDesc => query.OrderByDescending(c => c.Description),
                    _ => query.OrderBy(c => c.FirstName)
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying sorting.");
                throw new ApplicationException("An error occurred while applying sorting.", ex);
            }
        }

        /// <summary>
        /// Applies pagination to the client query based on the specified page number and page size.
        /// </summary>
        /// <param name="query">The query to paginate.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The paginated query.</returns>
        internal IQueryable<Client> ApplyPagination(IQueryable<Client> query, int page, int pageSize)
        {
            try
            {
                return query.Skip((page - 1) * pageSize).Take(pageSize);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying pagination.");
                throw new ApplicationException("An error occurred while applying pagination.", ex);
            }
        }
    }
}
