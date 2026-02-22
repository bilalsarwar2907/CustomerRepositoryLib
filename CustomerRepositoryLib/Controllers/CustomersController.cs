using Microsoft.AspNetCore.Mvc;
using CustomerRepositoryLib.Repositories;
using CustomerRepositoryLib.Models;

namespace CustomerRepositoryLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repo;

        // Dependency Injection: controller receives the repository
        public CustomersController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        // ---------------------------------------------------------
        // GET ALL CUSTOMERS
        // GET: api/customers
        // ---------------------------------------------------------
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repo.Get();
        }

        // ---------------------------------------------------------
        // GET CUSTOMER BY ID
        // GET: api/customers/5
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public ActionResult<Customer> Get(int id)
        {
            var customer = _repo.GetById(id);

            if (customer == null)
                return NotFound();

            return customer;
        }

        // ---------------------------------------------------------
        // CREATE CUSTOMER
        // POST: api/customers
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult<Customer> Post([FromBody] Customer customer)
        {
            var created = _repo.Add(customer);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // ---------------------------------------------------------
        // UPDATE CUSTOMER
        // PUT: api/customers/5
        // ---------------------------------------------------------
        [HttpPut("{id}")]
        public ActionResult<Customer> Put(int id, [FromBody] Customer customer)
        {
            var updated = _repo.Update(id, customer);

            if (updated == null)
                return NotFound();

            return updated;
        }

        // ---------------------------------------------------------
        // DELETE CUSTOMER
        // DELETE: api/customers/5
        // ---------------------------------------------------------
        [HttpDelete("{id}")]
        public ActionResult<Customer> Delete(int id)
        {
            var deleted = _repo.Delete(id);

            if (deleted == null)
                return NotFound();

            return deleted;
        }

        // ---------------------------------------------------------
        // SEARCH + FILTER + SORT
        // GET: api/customers/search?before=2000&after=1980&name=john&sortBy=name_desc
        // ---------------------------------------------------------
        [HttpGet("search")]
        public IEnumerable<Customer> Search(int? before, int? after, string? name, string? sortBy)
        {
            // If name is provided → use name search
            if (!string.IsNullOrEmpty(name))
                return _repo.GetByName(name);

            // Otherwise use filtering + sorting
            return _repo.Get(before, after, sortBy);
        }
    }
}