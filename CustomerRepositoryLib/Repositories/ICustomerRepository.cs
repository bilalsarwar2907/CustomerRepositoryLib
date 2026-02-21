using CustomerRepositoryLib.Models;

namespace CustomerRepositoryLib.Repositories
{
    public interface ICustomerRepository
    {
        // NEW: Extended Get method with filtering + sorting
        IEnumerable<Customer> Get(int? beforeYear = null, int? afterYear = null, string? sortBy = null);

        // Existing methods
        Customer? GetById(int id);
        Customer Add(Customer customer);
        Customer? Update(int id, Customer customer);
        Customer? Delete(int id);
    }
}