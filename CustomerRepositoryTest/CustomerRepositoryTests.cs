using CustomerRepositoryLib.Models;
using CustomerRepositoryLib.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerRepositoryTest
{
    public class CustomerRepositoryTests
    {
        // IMPORTANT CHANGE:
        // Instead of depending on the concrete class CustomerRepositoryList,
        // we now depend on the INTERFACE ICustomerRepository.
        // This allows us to reuse the SAME test class for:
        //   - List repository
        //   - Database repository
        //   - Any future repository type
        private ICustomerRepository _repo;

        private readonly bool useDatabase = true;

        //public CustomerRepositoryListTests()
        //{
        //    // Here we decide WHICH implementation to test.
        //    // For now, we test the LIST version.
        //    // Later, we can replace this with the database version
        //    // without changing ANY test code.
        //    _repo = new CustomerRepositoryList();
        //}
        public CustomerRepositoryTests()
        {
            if (useDatabase)
            {
                // 1. Build DbContextOptions with your connection string
                var options = new DbContextOptionsBuilder<CustomerDbContext>()
    .UseSqlServer("Server=CODE-PC\\SQLEXPRESS;Database=CustomerDB;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

                // 2. Create DbContext
                var context = new CustomerDbContext(options);

                // 3. TRUNCATE TABLE to ensure a clean database before each test
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE Customers");

                // 4. Use the DATABASE repository
                _repo = new CustomerRepositoryDatabase(context);
            }
            else
            {
                // Use the LIST repository
                _repo = new CustomerRepositoryList();
            }
        }

        [Fact]
        public void Get_ReturnsEmptyList_WhenNoCustomer()
        {
            // Act
            var result = _repo.Get();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Add_AddsCustomerSuccessfully()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "jh@mail.com",
                BirthYear = 1990
            };

            // Act
            var result = _repo.Add(customer);

            // Assert
            Assert.Equal(1, result.Id);          // ID should start at 1
            Assert.Single(_repo.Get());          // Only one customer in repo
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("jh@mail.com", result.Email);
            Assert.Equal(1990, result.BirthYear);
        }

        [Fact]
        public void GetById_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Yom Don",
                Email = "yd@mail.com",
                BirthYear = 1990
            };
            _repo.Add(customer);

            // Act
            var result = _repo.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Yom Don", result.Name);
            Assert.Equal("yd@mail.com", result.Email);
            Assert.Equal(1990, result.BirthYear);
        }

        [Fact]
        public void Delete_RemovesCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Jim Jon",
                Email = "jj@mail.com",
                BirthYear = 1990
            };
            _repo.Add(customer);

            // Act
            var result = _repo.Delete(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Jim Jon", result.Name);
            Assert.Equal("jj@mail.com", result.Email);
            Assert.Equal(1990, result.BirthYear);
            Assert.Empty(_repo.Get());   // Repository should now be empty
        }

        [Fact]
        public void Update_UpdatesCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Tim Jon",
                Email = "tm@mail.com",
                BirthYear = 1990
            };
            _repo.Add(customer);

            var updatedCustomer = new Customer
            {
                Name = "Tim Jon",
                Email = "tm@mail.com",
                BirthYear = 1999
            };

            // Act
            var result = _repo.Update(1, updatedCustomer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Tim Jon", result.Name);
            Assert.Equal("tm@mail.com", result.Email);
            Assert.Equal(1999, result.BirthYear);
        }
        [Fact]
        public void DatabaseConnection_Works()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseSqlServer("Server=CODE-PC\\SQLEXPRESS;Database=CustomerDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            var context = new CustomerDbContext(options);

            // Act + Assert
            context.Database.OpenConnection();
            context.Database.CloseConnection();
        }
    }
}