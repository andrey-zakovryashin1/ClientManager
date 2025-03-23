using Microsoft.EntityFrameworkCore;
using ClientManager.Models;

namespace ClientManager.Services
{
    /// <summary>
    /// Initializes the database with sample data if it is empty.
    /// </summary>
    public class DataInitializer
    {
        private readonly ClientsContext db;
        private readonly ILogger<DataInitializer> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInitializer"/> class.
        /// </summary>
        /// <param name="db">The database context for accessing client and address data.</param>
        /// <param name="logger">The logger for recording errors and information.</param>
        public DataInitializer(ClientsContext db, ILogger<DataInitializer> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        /// <summary>
        /// Initializes the database with sample data if no clients exist.
        /// </summary>
        public void Initialize()
        {
            try
            {

                if (!db.Clients.Any())
                {
                    var addresses = new List<Address>
                    {
                        new Address { StreetAddress = "123 Main St", City = "Anytown", State = "CA", Zip = "123450" },
                        new Address { StreetAddress = "456 Elm St", City = "Othertown", State = "NY", Zip = "678900" },
                        new Address { StreetAddress = "789 Oak St", City = "Springfield", State = "IL", Zip = "627010" },
                        new Address { StreetAddress = "101 Pine St", City = "Shelbyville", State = "IN", Zip = "461760" },
                        new Address { StreetAddress = "202 Maple St", City = "Capital City", State = "NV", Zip = "891010" },
                        new Address { StreetAddress = "303 Birch St", City = "Ogdenville", State = "UT", Zip = "844010" },
                        new Address { StreetAddress = "404 Cedar St", City = "North Haverbrook", State = "NH", Zip = "037840" },
                        new Address { StreetAddress = "505 Walnut St", City = "Brockway", State = "MI", Zip = "480970" },
                        new Address { StreetAddress = "606 Spruce St", City = "Springfield", State = "MO", Zip = "658020" },
                        new Address { StreetAddress = "707 Fir St", City = "Springfield", State = "OR", Zip = "974770" },
                        new Address { StreetAddress = "808 Pine St", City = "Shelbyville", State = "KY", Zip = "400650" },
                        new Address { StreetAddress = "909 Maple St", City = "Capital City", State = "TX", Zip = "733010" },
                        new Address { StreetAddress = "111 Birch St", City = "Ogdenville", State = "CO", Zip = "802020" },
                        new Address { StreetAddress = "222 Cedar St", City = "North Haverbrook", State = "VT", Zip = "056020" },
                        new Address { StreetAddress = "333 Walnut St", City = "Brockway", State = "WI", Zip = "530050" },
                        new Address { StreetAddress = "444 Spruce St", City = "Springfield", State = "MA", Zip = "011030" },
                        new Address { StreetAddress = "555 Fir St", City = "Springfield", State = "OH", Zip = "455020" },
                        new Address { StreetAddress = "666 Pine St", City = "Shelbyville", State = "TN", Zip = "371600" },
                        new Address { StreetAddress = "777 Maple St", City = "Capital City", State = "GA", Zip = "303010" },
                        new Address { StreetAddress = "888 Birch St", City = "Ogdenville", State = "FL", Zip = "320990" },
                        new Address { StreetAddress = "999 Cedar St", City = "North Haverbrook", State = "AZ", Zip = "850010" },
                        new Address { StreetAddress = "1010 Walnut St", City = "Brockway", State = "WA", Zip = "980040" },
                        new Address { StreetAddress = "1212 Spruce St", City = "Springfield", State = "PA", Zip = "171010" }
                    };

                    var clients = new List<Client>
                    {
                        new Client { FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123-456-7890", Address = addresses[0], Description = "Sample client 1" },
                        new Client { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Phone = "987-654-3210", Address = addresses[1], Description = "Sample client 2" },
                        new Client { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Phone = "555-123-4567", Address = addresses[2], Description = "Sample client 3" },
                        new Client { FirstName = "Bob", LastName = "Williams", Email = "bob@example.com", Phone = "555-987-6543", Address = addresses[3], Description = "Sample client 4" },
                        new Client { FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com", Phone = "555-555-5555", Address = addresses[4], Description = "Sample client 5" },
                        new Client { FirstName = "David", LastName = "Jones", Email = "david@example.com", Phone = "555-111-2222", Address = addresses[5], Description = "Sample client 6" },
                        new Client { FirstName = "Eve", LastName = "Garcia", Email = "eve@example.com", Phone = "555-333-4444", Address = addresses[6], Description = "Sample client 7" },
                        new Client { FirstName = "Frank", LastName = "Miller", Email = "frank@example.com", Phone = "555-666-7777", Address = addresses[7], Description = "Sample client 8" },
                        new Client { FirstName = "Grace", LastName = "Davis", Email = "grace@example.com", Phone = "555-888-9999", Address = addresses[8], Description = "Sample client 9" },
                        new Client { FirstName = "Hank", LastName = "Rodriguez", Email = "hank@example.com", Phone = "555-000-1111", Address = addresses[9], Description = "Sample client 10" },
                        new Client { FirstName = "Ivy", LastName = "Martinez", Email = "ivy@example.com", Phone = "555-222-3333", Address = addresses[10], Description = "Sample client 11" },
                        new Client { FirstName = "Jack", LastName = "Hernandez", Email = "jack@example.com", Phone = "555-444-5555", Address = addresses[11], Description = "Sample client 12" },
                        new Client { FirstName = "Karen", LastName = "Lopez", Email = "karen@example.com", Phone = "555-666-7777", Address = addresses[12], Description = "Sample client 13" },
                        new Client { FirstName = "Leo", LastName = "Gonzalez", Email = "leo@example.com", Phone = "555-888-9999", Address = addresses[13], Description = "Sample client 14" },
                        new Client { FirstName = "Mona", LastName = "Wilson", Email = "mona@example.com", Phone = "555-000-1111", Address = addresses[14], Description = "Sample client 15" },
                        new Client { FirstName = "Nina", LastName = "Anderson", Email = "nina@example.com", Phone = "555-222-3333", Address = addresses[15], Description = "Sample client 16" },
                        new Client { FirstName = "Oscar", LastName = "Thomas", Email = "oscar@example.com", Phone = "555-444-5555", Address = addresses[16], Description = "Sample client 17" },
                        new Client { FirstName = "Paul", LastName = "Taylor", Email = "paul@example.com", Phone = "555-666-7777", Address = addresses[17], Description = "Sample client 18" },
                        new Client { FirstName = "Quincy", LastName = "Moore", Email = "quincy@example.com", Phone = "555-888-9999", Address = addresses[18], Description = "Sample client 19" },
                        new Client { FirstName = "Rachel", LastName = "Jackson", Email = "rachel@example.com", Phone = "555-000-1111", Address = addresses[19], Description = "Sample client 20" },
                        new Client { FirstName = "Steve", LastName = "Martin", Email = "steve@example.com", Phone = "555-222-3333", Address = addresses[20], Description = "Sample client 21" },
                        new Client { FirstName = "Tina", LastName = "Lee", Email = "tina@example.com", Phone = "555-444-5555", Address = addresses[21], Description = "Sample client 22" },
                        new Client { FirstName = "Uma", LastName = "Perez", Email = "uma@example.com", Phone = "555-666-7777", Address = addresses[22], Description = "Sample client 23" }
                    };

                    db.Addresses.AddRange(addresses);
                    db.Clients.AddRange(clients);
                    db.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw new ApplicationException("An error occurred while initializing the database.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while initializing the database.");
                throw new ApplicationException("An unexpected error occurred while initializing the database.", ex);
            }
        }
    }
}
