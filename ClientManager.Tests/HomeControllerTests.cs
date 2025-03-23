using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ClientManager.Controllers;
using ClientManager.Models;
using ClientManager.Services;

namespace ClientManager.Tests
{
    /// <summary>
    /// Test class for the <see cref="HomeController"/>.
    /// Contains unit tests to verify the functionality of the <see cref="HomeController"/> methods.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<IClientService> mockClientService;
        private readonly Mock<ILogger<HomeController>> mockLogger;
        private readonly HomeController controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeControllerTests"/> class.
        /// Sets up the mock dependencies and initializes the <see cref="HomeController"/>.
        /// </summary>
        public HomeControllerTests()
        {
            mockClientService = new Mock<IClientService>();
            mockLogger = new Mock<ILogger<HomeController>>();
            controller = new HomeController(mockClientService.Object, mockLogger.Object);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.Index"/> method returns a <see cref="ViewResult"/> with a list of clients.
        /// </summary>
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            };

            mockClientService.Setup(service => service.GetClientsAsync(It.IsAny<string>(), It.IsAny<SortState>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(clients);

            mockClientService.Setup(service => service.GetClientsCountAsync(It.IsAny<string>()))
                .ReturnsAsync(clients.Count);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.Index(null, 1, SortState.FirstNameAsc);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Clients.Count());
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.Index"/> method returns a <see cref="JsonResult"/> when the "Accept" header is set to "application/json".
        /// </summary>
        [Fact]
        public async Task Index_ReturnsJsonResult_WhenAcceptHeaderIsApplicationJson()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" }
            };

            mockClientService.Setup(service => service.GetClientsAsync(It.IsAny<string>(), It.IsAny<SortState>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(clients);

            mockClientService.Setup(service => service.GetClientsCountAsync(It.IsAny<string>()))
                .ReturnsAsync(clients.Count);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Request.Headers["Accept"] = "application/json";

            // Act
            var result = await controller.Index(null, 1, SortState.FirstNameAsc);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.Index"/> method sorts clients by first name in ascending order.
        /// </summary>
        [Fact]
        public async Task Index_SortsClientsByFirstNameAsc()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new Client { Id = 2, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" },
                new Client { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com" }
            };

            mockClientService.Setup(service => service.GetClientsAsync(It.IsAny<string>(), SortState.FirstNameAsc, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(clients.OrderBy(c => c.FirstName).ToList());

            mockClientService.Setup(service => service.GetClientsCountAsync(It.IsAny<string>()))
                .ReturnsAsync(clients.Count);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.Index(null, 1, SortState.FirstNameAsc);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Alice", model.Clients.First().FirstName);
            Assert.Equal("John", model.Clients.Last().FirstName);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.Delete"/> method returns a JSON response indicating success when a client is deleted.
        /// </summary>
        [Fact]
        public async Task Delete_ReturnsJsonSuccess_WhenClientIsDeleted()
        {
            // Arrange
            mockClientService.Setup(service => service.DeleteClientAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.HttpContext.Request.Headers["Accept"] = "application/json";

            // Act
            var result = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonString = JsonConvert.SerializeObject(jsonResult.Value);
            var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            Assert.NotNull(jsonResponse);
            Assert.True(jsonResponse.Success);
            Assert.Equal("Client deleted successfully", jsonResponse.Message);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.Delete"/> method returns a JSON response indicating failure when the client is not found.
        /// </summary>
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenClientNotFound()
        {
            // Arrange
            mockClientService.Setup(service => service.DeleteClientAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Client not found"));

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.HttpContext.Request.Headers["Accept"] = "application/json";

            // Act
            var result = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonString = JsonConvert.SerializeObject(jsonResult.Value);
            var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            Assert.NotNull(jsonResponse);
            Assert.False(jsonResponse.Success);
            Assert.Equal("Client not found", jsonResponse.Message);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.EditClient"/> method returns a <see cref="ViewResult"/> with the client data.
        /// </summary>
        [Fact]
        public async Task EditClient_ReturnsViewResult_WithClient()
        {
            // Arrange
            var client = new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };

            mockClientService.Setup(service => service.GetClientByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(client);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.EditClient(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.EditClient"/> method redirects to the "Index" action after successfully updating a client.
        /// </summary>
        [Fact]
        public async Task EditClient_ReturnsRedirectToIndex_WhenClientIsUpdated()
        {
            // Arrange
            var client = new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };

            mockClientService.Setup(service => service.UpdateClientAsync(It.IsAny<Client>()))
                .Returns(Task.CompletedTask);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.EditClient(client);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.EditAddress"/> method returns a <see cref="ViewResult"/> with the address data.
        /// </summary>
        [Fact]
        public async Task EditAddress_ReturnsViewResult_WithAddress()
        {
            // Arrange
            var client = new Client { Id = 1, Address = new Address { Id = 1, StreetAddress = "123 Main St" } };

            mockClientService.Setup(service => service.GetClientByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(client);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.EditAddress(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Address>(viewResult.Model);
            Assert.Equal("123 Main St", model.StreetAddress);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.EditAddress"/> method redirects to the "Index" action after successfully updating an address.
        /// </summary>
        [Fact]
        public async Task EditAddress_ReturnsRedirectToIndex_WhenAddressIsUpdated()
        {
            // Arrange
            var address = new Address
            {
                Id = 1,
                StreetAddress = "123 Main St",
                City = "Anytown",
                State = "CA",
                Zip = "123456"
            };

            mockClientService.Setup(service => service.UpdateAddressAsync(It.IsAny<Address>()))
                .Returns(Task.CompletedTask);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.EditAddress(address);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        /// <summary>
        /// Tests that the <see cref="HomeController.HandleException"/> method returns a JSON response when the "Accept" header is set to "application/json".
        /// </summary>
        [Fact]
        public void HandleException_ReturnsJson_WhenAcceptHeaderIsApplicationJson()
        {
            // Arrange
            var exception = new Exception("Test exception");

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.HttpContext.Request.Headers["Accept"] = "application/json";

            // Act
            var result = controller.HandleException(exception);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonString = JsonConvert.SerializeObject(jsonResult.Value);
            var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            Assert.NotNull(jsonResponse);
            Assert.False(jsonResponse.Success);
            Assert.Equal("An unexpected error occurred.", jsonResponse.Message);
        }
    }
}