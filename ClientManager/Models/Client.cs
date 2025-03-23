using System.ComponentModel.DataAnnotations;

namespace ClientManager.Models
{
    /// <summary>
    /// Represents a client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The unique identifier for the client.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Client ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Client ID must be a positive number.")]
        public int Id { get; set; }

        /// <summary>
        /// The first name of the client. Must contain only letters.
        /// </summary>
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First Name should contain only letters")]
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the client. Must contain only letters.
        /// </summary>
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Last Name should contain only letters")]
        public string? LastName { get; set; }

        /// <summary>
        /// The email address of the client. Must be a valid email format.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        /// <summary>
        /// The phone number of the client. Must be a valid phone number format.
        /// </summary>
        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string? Phone { get; set; }

        /// <summary>
        /// The address of the client.
        /// </summary>
        public Address? Address { get; set; }

        /// <summary>
        /// A description of the client.
        /// </summary>
        public string? Description { get; set; }

    }
}
