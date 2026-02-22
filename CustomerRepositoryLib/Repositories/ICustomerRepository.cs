using CustomerRepositoryLib.Models;

namespace CustomerRepositoryLib.Repositories
{
    public interface ICustomerRepository
    {
        // Basic CRUD
        IEnumerable<Customer> Get();
        Customer? GetById(int id);
        Customer Add(Customer customer);
        Customer? Delete(int id);
        Customer? Update(int id, Customer updatedCustomer);

        // Filtering
        IEnumerable<Customer> Get(int? yearBefore);
        IEnumerable<Customer> Get(int? yearBefore, int? yearAfter);
        IEnumerable<Customer> GetByName(string? name);

        // Filtering + Sorting
        IEnumerable<Customer> Get(int? beforeYear, int? afterYear, string? sortBy);
    }
}