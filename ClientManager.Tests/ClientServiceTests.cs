using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ClientManager.Models;
using ClientManager.Services;

namespace ClientManager.Tests
{
    /// <summary>
    /// Test class for the <see cref="ClientService"/>.
    /// Contains unit tests to verify the functionality of the <see cref="ClientService"/> methods.
    /// </summary>
    public class ClientServiceTests : IDisposable
    {
        private readonly DbContextOptions<ClientsContext> options;
        private readonly ClientsContext context;
        private readonly ClientQueryService clientQueryService;
        private readonly ClientService clientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientServiceTests"/> class.
        /// Sets up the in-memory database and initializes the <see cref="ClientService"/> and <see cref="ClientQueryService"/>.
        /// </summary>
        public ClientServiceTests()
        {
            options = new DbContextOptionsBuilder<ClientsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new ClientsContext(options);
            var mockLoggerClientService = new Mock<ILogger<ClientService>>();
            var mockLoggerClientQueryService = new Mock<ILogger<ClientQueryService>>();
            clientQueryService = new ClientQueryService(context, mockLoggerClientQueryService.Object);
            clientService = new ClientService(context, clientQueryService, mockLoggerClientService.Object);
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.GetClientByIdAsync"/> method returns the correct client by ID.
        /// </summary>
        [Fact]
        public async Task GetClientByIdAsync_ReturnsClient()
        {
            // Arrange
            var address = new Address { StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" };
            var client = new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123-456-7890", Address = address, Description = "Sample client 1" };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            // Act
            var result = await clientService.GetClientByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.GetClientsAsync"/> method returns a list of clients filtered by last name.
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
            var result = await clientService.GetClientsAsync("Doe", SortState.LastNameAsc, 1, 10);

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.GetClientsCountAsync"/> method returns the correct count of clients filtered by last name.
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
            var result = await clientService.GetClientsCountAsync("Doe");

            // Assert
            Assert.Equal(2, result);
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.DeleteClientAsync"/> method deletes a client by ID.
        /// </summary>
        [Fact]
        public async Task DeleteClientAsync_DeletesClient()
        {
            // Arrange
            var address = new Address { StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" };
            var client = new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123-456-7890", Address = address, Description = "Sample client 1" };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            // Act
            await clientService.DeleteClientAsync(1);

            // Assert
            var deletedClient = await context.Clients.FindAsync(1);
            Assert.Null(deletedClient);
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.DeleteClientAsync"/> method throws a <see cref="KeyNotFoundException"/> when attempting to delete a non-existent client.
        /// </summary>
        [Fact]
        public async Task DeleteClientAsync_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentClientId = 999;

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => clientService.DeleteClientAsync(nonExistentClientId));
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.UpdateClientAsync"/> method updates an existing client.
        /// </summary>
        [Fact]
        public async Task UpdateClientAsync_UpdatesClient()
        {
            // Arrange
            var address = new Address { StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" };
            var client = new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123-456-7890", Address = address, Description = "Sample client 1" };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            var updatedClient = new Client { Id = 1, FirstName = "Johnny", LastName = "Doe", Email = "johnny@example.com", Phone = "123-456-7890", Address = address, Description = "Updated client 1" };

            // Act
            await clientService.UpdateClientAsync(updatedClient);

            // Assert
            var result = await context.Clients.FindAsync(1);
            Assert.Equal("Johnny", result.FirstName);
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.UpdateClientAsync"/> method throws a <see cref="KeyNotFoundException"/> when attempting to update a non-existent client.
        /// </summary>
        [Fact]
        public async Task UpdateClientAsync_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentClient = new Client { Id = 999, FirstName = "Non", LastName = "Existent", Email = "nonexistent@example.com", Phone = "000-000-0000" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => clientService.UpdateClientAsync(nonExistentClient));
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.UpdateAddressAsync"/> method updates an existing address.
        /// </summary>
        [Fact]
        public async Task UpdateAddressAsync_UpdatesAddress()
        {
            // Arrange
            var address = new Address { Id = 1, StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" };
            context.Addresses.Add(address);
            await context.SaveChangesAsync();

            var updatedAddress = new Address { Id = 1, StreetAddress = "456 Elm St", City = "Othertown", State = "NY", Zip = "678900" };

            // Act
            await clientService.UpdateAddressAsync(updatedAddress);

            // Assert
            var result = await context.Addresses.FindAsync(1);
            Assert.Equal("456 Elm St", result.StreetAddress);
        }

        /// <summary>
        /// Tests that the <see cref="ClientService.UpdateAddressAsync"/> method throws a <see cref="KeyNotFoundException"/> when attempting to update a non-existent address.
        /// </summary>
        [Fact]
        public async Task UpdateAddressAsync_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentAddress = new Address { Id = 999, StreetAddress = "Non Existent St", City = "Nowhere", State = "XX", Zip = "000000" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => clientService.UpdateAddressAsync(nonExistentAddress));
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

