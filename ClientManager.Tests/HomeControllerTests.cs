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
    public class HomeControllerTests
    {
        private readonly Mock<IClientService> mockClientService;
        private readonly Mock<ILogger<HomeController>> mockLogger;
        private readonly HomeController controller;

        public HomeControllerTests()
        {
            mockClientService = new Mock<IClientService>();
            mockLogger = new Mock<ILogger<HomeController>>();
            controller = new HomeController(mockClientService.Object, mockLogger.Object);
        }

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