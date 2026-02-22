using CustomerRepositoryLib.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerRepositoryLib.Repositories
{
    /// <summary>
    /// A database-backed implementation of ICustomerRepository.
    /// This repository uses Entity Framework Core to perform CRUD
    /// operations on the Customers table in a SQL database.
    ///
    /// This class mirrors the structure of the List-based repository,
    /// but all operations are executed against a real database.
    /// </summary>
    public class CustomerRepositoryDatabase : ICustomerRepository
    {
        /// <summary>
        /// EF Core DbContext used to access the database.
        /// Injected via dependency injection.
        /// </summary>
        private readonly CustomerDbContext _context;

        /// <summary>
        /// Constructor that receives the DbContext from DI.
        /// </summary>
        public CustomerRepositoryDatabase(CustomerDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // BASIC CRUD OPERATIONS
        // ---------------------------------------------------------

        /// <summary>
        /// Returns all customers from the database.
        /// </summary>
        public IEnumerable<Customer> Get()
        {
            return _context.Customers.ToList();
        }

        /// <summary>
        /// Returns a customer by ID, or null if not found.
        /// </summary>
        public Customer? GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Adds a new customer to the database.
        /// EF Core automatically assigns the ID if configured.
        /// </summary>
        public Customer Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer;
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// Returns the deleted customer or null if not found.
        /// </summary>
        public Customer? Delete(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
                return null;

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return customer;
        }

        /// <summary>
        /// Updates an existing customer.
        /// Returns the updated customer or null if not found.
        /// </summary>
        public Customer? Update(int id, Customer updatedCustomer)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
                return null;

            customer.Name = updatedCustomer.Name;
            customer.Email = updatedCustomer.Email;
            customer.BirthYear = updatedCustomer.BirthYear;

            _context.SaveChanges();

            return customer;
        }

        // ---------------------------------------------------------
        // FILTERING METHODS
        // ---------------------------------------------------------

        /// <summary>
        /// Returns customers born before a given year.
        /// If yearBefore is null, returns all customers.
        /// </summary>
        public IEnumerable<Customer> Get(int? yearBefore)
        {
            if (yearBefore == null)
                return _context.Customers.ToList();

            return _context.Customers
                .Where(c => c.BirthYear < yearBefore.Value)
                .ToList();
        }

        /// <summary>
        /// Returns customers born between two years.
        /// </summary>
        public IEnumerable<Customer> Get(int? yearBefore, int? yearAfter)
        {
            IQueryable<Customer> result = _context.Customers;

            if (yearBefore != null)
                result = result.Where(c => c.BirthYear < yearBefore.Value);

            if (yearAfter != null)
                result = result.Where(c => c.BirthYear > yearAfter.Value);

            return result.ToList();
        }

        /// <summary>
        /// Returns customers whose name or email contains the search term.
        /// Case-insensitive.
        /// </summary>
        public IEnumerable<Customer> GetByName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return _context.Customers.ToList();

            string lower = name.ToLower();

            return _context.Customers
                .Where(c =>
                    c.Name.ToLower().Contains(lower) ||
                    c.Email.ToLower().Contains(lower))
                .ToList();
        }

        // ---------------------------------------------------------
        // FILTER + SORT (FULL COMBINED METHOD)
        // ---------------------------------------------------------

        /// <summary>
        /// Returns customers filtered by birth year and sorted by a given field.
        /// Supported sort options:
        /// id, id_desc
        /// name, name_desc
        /// email, email_desc
        /// birthyear, birthyear_desc
        /// </summary>
        public IEnumerable<Customer> Get(int? beforeYear, int? afterYear, string? sortBy)
        {
            IQueryable<Customer> result = _context.Customers;

            // Filtering
            if (beforeYear != null)
                result = result.Where(c => c.BirthYear < beforeYear.Value);

            if (afterYear != null)
                result = result.Where(c => c.BirthYear > afterYear.Value);

            // Sorting
            result = sortBy switch
            {
                "id" => result.OrderBy(c => c.Id),
                "id_desc" => result.OrderByDescending(c => c.Id),

                "name" => result.OrderBy(c => c.Name),
                "name_desc" => result.OrderByDescending(c => c.Name),

                "email" => result.OrderBy(c => c.Email),
                "email_desc" => result.OrderByDescending(c => c.Email),

                "birthyear" => result.OrderBy(c => c.BirthYear),
                "birthyear_desc" => result.OrderByDescending(c => c.BirthYear),

                _ => result
            };

            return result.ToList();
        }
    }
}