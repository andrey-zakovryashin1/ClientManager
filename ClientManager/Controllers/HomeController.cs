using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using ClientManager.Models;
using ClientManager.Services;

[assembly: InternalsVisibleTo("ClientManager.Tests")]

namespace ClientManager.Controllers
{
    /// <summary>
    /// The HomeController handles requests related to managing clients and their addresses.
    /// It provides actions for displaying, filtering, sorting, paginating, editing and deleting clients.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IClientService clientService;
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="clientService">The service responsible for client-related operations.</param>
        /// <param name="logger">The logger used for logging errors and information.</param>

        public HomeController(IClientService clientService, ILogger<HomeController> logger)
        {
            this.clientService = clientService;
            this.logger = logger;

        }

        /// <summary>
        /// Displays a paginated and sorted list of clients optionally filtered by a search term.
        /// Supports both HTML and JSON responses.
        /// </summary>
        /// <param name="filterText">The search term used to filter clients by name, email, phone or address.</param>
        /// <param name="page">The current page number for pagination (default is 1).</param>
        /// <param name="sortOrder">The sorting criteria for the client list (default is ascending by first name).</param>
        /// <returns>
        /// A view containing the filtered, sorted and paginated list of clients or a JSON response if requested.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs while retrieving clients.
        /// </exception>
        [HttpGet]
        public async Task<IActionResult> Index(string filterText, int page = 1, SortState sortOrder = SortState.FirstNameAsc)
        {
            try
            {
                int pageSize = 10;
                var clients = await clientService.GetClientsAsync(filterText, sortOrder, page, pageSize);
                var count = await clientService.GetClientsCountAsync(filterText);

                var viewModel = new IndexViewModel(
                    clients,
                    new PageViewModel(count, page, pageSize),
                    new FilterViewModel(filterText),
                    new SortViewModel(sortOrder)
                );

                if (Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    var result = clients.Select(c => new
                    {
                        id = c.Id,
                        firstName = c.FirstName,
                        lastName = c.LastName,
                        email = c.Email,
                        phone = c.Phone,
                        address = new
                        {
                            streetAddress = c.Address?.StreetAddress,
                            city = c.Address?.City,
                            state = c.Address?.State,
                            zip = c.Address?.Zip
                        },
                        description = c.Description
                    });

                    return Json(result);
                }

                ViewBag.FilterApplied = !string.IsNullOrEmpty(filterText);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading the index page.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Deletes a client by its ID. Supports both HTML and JSON responses.
        /// </summary>
        /// <param name="id">The ID of the client to delete.</param>
        /// <returns>
        /// A JSON response indicating success or failure, or a redirect to the Index page for HTML requests.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the client with the specified ID is not found.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs while deleting the client.
        /// </exception>
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "Client ID is null" })
                        : NotFound();
                }

                await clientService.DeleteClientAsync(id.Value);

                return Request.Headers["Accept"].ToString().Contains("application/json")
                    ? Json(new { success = true, message = "Client deleted successfully" })
                    : RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                return Request.Headers["Accept"].ToString().Contains("application/json")
                    ? Json(new { success = false, message = ex.Message })
                    : NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting a client.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Displays the form for editing client's details. Supports both HTML and JSON responses.
        /// </summary>
        /// <param name="id">The ID of the client to edit.</param>
        /// <returns>
        /// A view containing the edit form or a JSON response with client's details if requested.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs while retrieving the client.
        /// </exception>
        [HttpGet]
        public async Task<IActionResult> EditClient(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "ID is null" })
                        : NotFound();
                }

                var client = await clientService.GetClientByIdAsync(id.Value);
                if (client == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "Client not found" })
                        : NotFound();
                }

                if (Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    var result = new
                    {
                        id = client.Id,
                        firstName = client.FirstName,
                        lastName = client.LastName,
                        email = client.Email,
                        phone = client.Phone,
                        address = new
                        {
                            streetAddress = client.Address?.StreetAddress,
                            city = client.Address?.City,
                            state = client.Address?.State,
                            zip = client.Address?.Zip
                        },
                        description = client.Description
                    };

                    return Json(new { success = true, data = result });
                }

                return View(client);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading the edit client page.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Updates client's details based on the submitted form data. Supports both HTML and JSON responses.
        /// </summary>
        /// <param name="client">The client object containing updated details.</param>
        /// <returns>
        /// A JSON response indicating success or failure, or a redirect to the Index page for HTML requests.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the client with the specified ID is not found.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs while updating the client.
        /// </exception>
        [HttpPost]
        public async Task<IActionResult> EditClient(Client client)
        {
            try
            {
                if (client == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "Client data is null" })
                        : BadRequest();
                }

                // The Address.Id validation check is not performed because this field is not edited by the user and its validation is not required
                ModelState.Remove("Address.Id");

                if (!ModelState.IsValid)
                {
                    if (Request.Headers["Accept"].ToString().Contains("application/json"))
                    {
                        var errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => new
                            {
                                errorMessage = e.ErrorMessage,
                                exception = e.Exception?.Message
                            })
                            .ToList();

                        return Json(new { success = false, message = "Invalid data", errors });
                    }

                    return View(client);
                }

                await clientService.UpdateClientAsync(client);

                if (Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    var result = new
                    {
                        id = client.Id,
                        firstName = client.FirstName,
                        lastName = client.LastName,
                        email = client.Email,
                        phone = client.Phone,
                        address = new
                        {
                            streetAddress = client.Address?.StreetAddress,
                            city = client.Address?.City,
                            state = client.Address?.State,
                            zip = client.Address?.Zip
                        },
                        description = client.Description
                    };

                    return Json(new { success = true, data = result });
                }

                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                return Request.Headers["Accept"].ToString().Contains("application/json")
                    ? Json(new { success = false, message = ex.Message })
                    : NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating a client.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Displays the form for editing a client's address. Supports both HTML and JSON responses.
        /// </summary>
        /// <param name="id">The ID of the client whose address is being edited.</param>
        /// <returns>
        /// A view containing the edit form or a JSON response with the address details if requested.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs while retrieving the client's address.
        /// </exception>
        [HttpGet]
        public async Task<IActionResult> EditAddress(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "ID is null" })
                        : NotFound();
                }

                var client = await clientService.GetClientByIdAsync(id.Value);
                if (client == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "Client not found" })
                        : NotFound();
                }

                if (Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    var result = new
                    {
                        id = client.Id,
                        address = new
                        {
                            streetAddress = client.Address?.StreetAddress,
                            city = client.Address?.City,
                            state = client.Address?.State,
                            zip = client.Address?.Zip
                        }
                    };

                    return Json(new { success = true, data = result });
                }

                return View(client.Address);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading the edit address page.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Updates a client's address based on the submitted form data. Supports both HTML and JSON responses.
        /// </summary>
        /// <param name="address">The address object containing updated details.</param>
        /// <returns>
        /// A JSON response indicating success or failure, or a redirect to the Index page for HTML requests.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the address with the specified ID is not found.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs while updating the address.
        /// </exception>
        [HttpPost]
        public async Task<IActionResult> EditAddress(Address address)
        {
            try
            {
                if (address == null)
                {
                    return Request.Headers["Accept"].ToString().Contains("application/json")
                        ? Json(new { success = false, message = "Address data is null" })
                        : BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    if (Request.Headers["Accept"].ToString().Contains("application/json"))
                    {
                        var errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => new
                            {
                                errorMessage = e.ErrorMessage,
                                exception = e.Exception?.Message
                            })
                            .ToList();

                        return Json(new { success = false, message = "Invalid data", errors });
                    }

                    return View(address);
                }

                await clientService.UpdateAddressAsync(address);

                if (Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    var result = new
                    {
                        streetAddress = address.StreetAddress,
                        city = address.City,
                        state = address.State,
                        zip = address.Zip
                    };

                    return Json(new { success = true, data = result });
                }

                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                return Request.Headers["Accept"].ToString().Contains("application/json")
                    ? Json(new { success = false, message = ex.Message })
                    : NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating an address.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Handles exceptions and returns an appropriate response (JSON or error page).
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>A JSON response with an error message or a 500 error page.</returns>
        internal IActionResult HandleException(Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");

            if (Request?.Headers != null && Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }

            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }
}