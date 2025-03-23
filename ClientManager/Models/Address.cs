using System.ComponentModel.DataAnnotations;

namespace ClientManager.Models
{
    /// <summary>
    /// Represents the address of a client.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The unique identifier for the address.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Address ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Address ID must be a positive number.")]
        public int Id { get; set; }

        /// <summary>
        /// The street address, including the house number.
        /// </summary>
        [Required(ErrorMessage = "Street Address is required")]
        public string? StreetAddress { get; set; }

        /// <summary>
        /// The city where the address is located.
        /// </summary>
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }

        /// <summary>
        /// The state where the address is located.
        /// </summary>
        [Required(ErrorMessage = "State is required")]
        public string? State { get; set; }

        /// <summary>
        /// The postal code for the address. Must be exactly 6 digits.
        /// </summary>
        [Required(ErrorMessage = "ZIP is required")]
        [RegularExpression("^\\d{6}$", ErrorMessage = "ZIP must be 6 digits")]
        public string? Zip { get; set; }
    }
}
