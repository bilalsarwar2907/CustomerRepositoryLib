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

        public IEnumerable<Customer> Get()
        {
            return _customers;
        }

        public Customer? GetById(int id)
        {
            return _customers.FirstOrDefault(customer => customer.Id == id);
        }
        public Customer? Add(Customer customer)
        {
            customer.Id = _nextId++;
            _customers.Add(customer);
            return customer;
        }
        public Customer? Delete(int id)
        {
            var customer = GetById(id);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
            return customer;
        }
        public Customer? Update(int id, Customer updatedCustomer)
        {
            var customer = GetById(id);
            if (customer != null)
            {
                customer.Name = updatedCustomer.Name;
                customer.Email = updatedCustomer.Email;
                customer.BirthYear = updatedCustomer.BirthYear;
            }
            return customer;
        }
    }
}
