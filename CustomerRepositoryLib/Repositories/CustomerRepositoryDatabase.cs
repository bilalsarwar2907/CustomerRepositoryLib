using CustomerRepositoryLib.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerRepositoryLib.Repositories
{
    // This class is the DATABASE version of your repository.
    // It implements the SAME interface as the List version.
    public class CustomerRepositoryDatabase : ICustomerRepository
    {
        // EF Core DbContext injected through the constructor.
        // This is required by the assignment (dependency injection).
        private readonly CustomerDbContext _context;

        public CustomerRepositoryDatabase(CustomerDbContext context)
        {
            _context = context;
        }

        // GET ALL CUSTOMERS
        public IEnumerable<Customer> Get()
        {
            // Reads all rows from the Customers table
            return _context.Customers.ToList();
        }

        // GET CUSTOMER BY ID
        public Customer? GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        // ADD CUSTOMER
        public Customer Add(Customer customer)
        {
            // Add to EF Core tracking
            _context.Customers.Add(customer);

            // Save to database (required!)
            _context.SaveChanges();

            return customer;
        }

        // DELETE CUSTOMER
        public Customer? Delete(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
                return null;

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return customer;
        }

        // UPDATE CUSTOMER
        public Customer? Update(int id, Customer updatedCustomer)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
                return null;

            // Update fields
            customer.Name = updatedCustomer.Name;
            customer.Email = updatedCustomer.Email;
            customer.BirthYear = updatedCustomer.BirthYear;

            _context.SaveChanges();

            return customer;
        }
    }
}