using CustomerRepositoryLib.Models;

namespace CustomerRepositoryLib.Repositories
{
    public class CustomerRepositoryList : ICustomerRepository
    {
        private readonly List<Customer> _customers = new List<Customer>();
        private int _nextId = 1;

        public CustomerRepositoryList()
        {
        }

        // ---------------------------------------------------------
        // BASIC CRUD
        // ---------------------------------------------------------

        /// <summary>
        /// Returns all customers.
        /// </summary>
        public IEnumerable<Customer> Get()
        {
            return _customers;
        }

        /// <summary>
        /// Returns a customer by ID, or null if not found.
        /// </summary>
        public Customer? GetById(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Adds a new customer and assigns a unique ID.
        /// </summary>
        public Customer? Add(Customer customer)
        {
            customer.Id = _nextId++;
            _customers.Add(customer);
            return customer;
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// Returns the deleted customer or null if not found.
        /// </summary>
        public Customer? Delete(int id)
        {
            var customer = GetById(id);
            if (customer != null)
                _customers.Remove(customer);

            return customer;
        }

        /// <summary>
        /// Updates an existing customer.
        /// Returns the updated customer or null if not found.
        /// </summary>
        public Customer? Update(int id, Customer updatedCustomer)
        {
            var customer = GetById(id);
            if (customer == null)
                return null;

            customer.Name = updatedCustomer.Name;
            customer.Email = updatedCustomer.Email;
            customer.BirthYear = updatedCustomer.BirthYear;

            return customer;
        }

        // ---------------------------------------------------------
        // FILTERING
        // ---------------------------------------------------------

        /// <summary>
        /// Returns customers born before a given year.
        /// If yearBefore is null, returns all customers.
        /// </summary>
        public IEnumerable<Customer> Get(int? yearBefore)
        {
            if (yearBefore == null)
                return _customers;

            return _customers
                .Where(c => c.BirthYear < yearBefore.Value)
                .ToList();
        }

        /// <summary>
        /// Returns customers born between two years.
        /// </summary>
        public IEnumerable<Customer> Get(int? yearBefore, int? yearAfter)
        {
            IEnumerable<Customer> result = _customers;

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
                return _customers;

            string lower = name.ToLower();

            return _customers
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
            // 1. Start with all customers
            IEnumerable<Customer> result = _customers;

            // 2. Apply filtering
            if (beforeYear != null)
                result = result.Where(c => c.BirthYear < beforeYear.Value);

            if (afterYear != null)
                result = result.Where(c => c.BirthYear > afterYear.Value);

            // 3. Apply sorting
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

            // 4. Return final list
            return result.ToList();
        }
    }
}