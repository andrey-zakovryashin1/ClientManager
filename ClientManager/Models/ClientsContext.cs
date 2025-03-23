using Microsoft.EntityFrameworkCore;

namespace ClientManager.Models
{
    /// <summary>
    /// The database context for managing clients and their addresses.
    /// </summary>
    public class ClientsContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet for the Clients table in the database.
        /// </summary>
        public DbSet<Client> Clients { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for the Addresses table in the database.
        /// </summary>
        public DbSet<Address> Addresses { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsContext"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public ClientsContext(DbContextOptions<ClientsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
    }
}
