    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using ClientManager.Models;
    using ClientManager.Services;

    namespace ClientManager.Tests
    {
    /// <summary>
    /// Test class for the <see cref="ClientQueryService"/>.
    /// Contains unit tests to verify the functionality of the <see cref="ClientQueryService"/>. methods.
    /// </summary>
    public class ClientQueryServiceTests : IDisposable
    {
        private readonly DbContextOptions<ClientsContext> options;
        private readonly ClientsContext context;
        private readonly ClientQueryService clientQueryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQueryServiceTests"/> class.
        /// Sets up the in-memory database and initializes the <see cref="ClientQueryService"/>.
        /// </summary>
        public ClientQueryServiceTests()
        {
            options = new DbContextOptionsBuilder<ClientsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new ClientsContext(options);
            var mockLogger = new Mock<ILogger<ClientQueryService>>();
            clientQueryService = new ClientQueryService(context, mockLogger.Object);
        }

        /// <summary>
        /// Tests that the <see cref="ClientQueryService.GetClientsAsync"/> method returns the correct list of clients.
        /// </summary>
        [Fact]
        public async Task GetClientsAsync_ReturnsClients()
        {
            // Arrange
            var address1 = new Address { StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" };
            var address2 = new Address { StreetAddress = "456 Elm St", City = "Othertown", State = "NY", Zip = "678900" };
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123-456-7890", Address = address1, Description = "Sample client 1" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Phone = "987-654-3210", Address = address2, Description = "Sample client 2" }
            };
            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();

            // Act
            var result = await clientQueryService.GetClientsAsync("Doe", SortState.LastNameAsc, 1, 10);

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that the <see cref="ClientQueryService.GetClientsCountAsync"/> method returns the correct count of clients.
        /// </summary>
        [Fact]
        public async Task GetClientsCountAsync_ReturnsCount()
        {
            // Arrange
            var address1 = new Address { StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" };
            var address2 = new Address { StreetAddress = "456 Elm St", City = "Othertown", State = "NY", Zip = "678900" };
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123-456-7890", Address = address1, Description = "Sample client 1" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Phone = "987-654-3210", Address = address2, Description = "Sample client 2" }
            };
            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();

            // Act
            var result = await clientQueryService.GetClientsCountAsync("Doe");

            // Assert
            Assert.Equal(2, result);
        }

        /// <summary>
        /// Tests that the <see cref="ClientQueryService.ApplySorting"/> method correctly sorts clients.
        /// </summary>
        [Fact]
        public void ApplySorting_SortsClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Smith" }
            }.AsQueryable();

            // Act
            var result = clientQueryService.ApplySorting(clients, SortState.LastNameAsc);

            // Assert
            Assert.Equal("Doe", result.First().LastName);
        }

        /// <summary>
        /// Tests that the <see cref="ClientQueryService.ApplyPagination"/> method correctly paginates clients.
        /// </summary>
        [Fact]
        public void ApplyPagination_PaginatesClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Smith" },
                new Client { Id = 3, FirstName = "Alice", LastName = "Johnson" }
            }.AsQueryable();

            // Act
            var result = clientQueryService.ApplyPagination(clients, 2, 1);

            // Assert
            Assert.Single(result);
            Assert.Equal("Jane", result.First().FirstName);
        }

        /// <summary>
        /// Cleans up the in-memory database after each test.
        /// </summary>
        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
