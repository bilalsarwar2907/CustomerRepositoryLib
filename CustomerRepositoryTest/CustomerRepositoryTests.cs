using CustomerRepositoryLib.Models;
using CustomerRepositoryLib.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerRepositoryTest
{
    public class CustomerRepositoryTests
    {
        private ICustomerRepository _repo;
        private readonly bool useDatabase = true;

        public CustomerRepositoryTests()
        {
            if (useDatabase)
            {
                var options = new DbContextOptionsBuilder<CustomerDbContext>()
                    .UseSqlServer("Server=CODE-PC\\SQLEXPRESS;Database=CustomerDB;Trusted_Connection=True;TrustServerCertificate=True;")
                    .Options;

                var context = new CustomerDbContext(options);

                context.Database.ExecuteSqlRaw("TRUNCATE TABLE Customers");

                _repo = new CustomerRepositoryDatabase(context);
            }
            else
            {
                _repo = new CustomerRepositoryList();
            }
        }

        // ---------------------------------------------------------
        // BASIC CRUD TESTS
        // ---------------------------------------------------------

        [Fact]
        public void Get_ReturnsEmptyList_WhenNoCustomer()
        {
            var result = _repo.Get();
            Assert.Empty(result);
        }

        [Fact]
        public void Add_AddsCustomerSuccessfully()
        {
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "jh@mail.com",
                BirthYear = 1990
            };

            var result = _repo.Add(customer);

            Assert.Equal(1, result.Id);
            Assert.Single(_repo.Get());
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("jh@mail.com", result.Email);
            Assert.Equal(1990, result.BirthYear);
        }

        [Fact]
        public void GetById_ReturnsCustomer_WhenCustomerExists()
        {
            var customer = new Customer
            {
                Name = "Yom Don",
                Email = "yd@mail.com",
                BirthYear = 1990
            };
            _repo.Add(customer);

            var result = _repo.GetById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Yom Don", result.Name);
            Assert.Equal("yd@mail.com", result.Email);
            Assert.Equal(1990, result.BirthYear);
        }

        [Fact]
        public void Delete_RemovesCustomer_WhenCustomerExists()
        {
            var customer = new Customer
            {
                Name = "Jim Jon",
                Email = "jj@mail.com",
                BirthYear = 1990
            };
            _repo.Add(customer);

            var result = _repo.Delete(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Jim Jon", result.Name);
            Assert.Equal("jj@mail.com", result.Email);
            Assert.Equal(1990, result.BirthYear);
            Assert.Empty(_repo.Get());
        }

        [Fact]
        public void Update_UpdatesCustomer_WhenCustomerExists()
        {
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

            var result = _repo.Update(1, updatedCustomer);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Tim Jon", result.Name);
            Assert.Equal("tm@mail.com", result.Email);
            Assert.Equal(1999, result.BirthYear);
        }

        // ---------------------------------------------------------
        // FILTERING TESTS
        // ---------------------------------------------------------

        [Fact]
        public void Get_Filter_BornBeforeYear()
        {
            _repo.Add(new Customer { Name = "A", Email = "a@mail.com", BirthYear = 1980 });
            _repo.Add(new Customer { Name = "B", Email = "b@mail.com", BirthYear = 2000 });

            var result = _repo.Get(1990); // before 1990

            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("A", list[0].Name);
        }

        [Fact]
        public void Get_Filter_BetweenYears()
        {
            _repo.Add(new Customer { Name = "A", Email = "a@mail.com", BirthYear = 1980 });
            _repo.Add(new Customer { Name = "B", Email = "b@mail.com", BirthYear = 1995 });
            _repo.Add(new Customer { Name = "C", Email = "c@mail.com", BirthYear = 2010 });

            var result = _repo.Get(2000, 1985); // >1985 and <2000

            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("B", list[0].Name);
        }

        [Fact]
        public void GetByName_ReturnsMatchingCustomers()
        {
            _repo.Add(new Customer { Name = "Alice Johnson", Email = "alice@mail.com", BirthYear = 1990 });
            _repo.Add(new Customer { Name = "Bob Alice", Email = "bob@mail.com", BirthYear = 1992 });
            _repo.Add(new Customer { Name = "Charlie", Email = "charlie@mail.com", BirthYear = 1995 });

            var result = _repo.GetByName("alice");

            Assert.Equal(2, result.Count());
        }

        // ---------------------------------------------------------
        // SORTING TESTS (WITH FILTER COMBINED)
        // ---------------------------------------------------------

        [Fact]
        public void Get_SortBy_Id_Ascending()
        {
            _repo.Add(new Customer { Name = "B", Email = "b@mail.com", BirthYear = 2000 });
            _repo.Add(new Customer { Name = "A", Email = "a@mail.com", BirthYear = 1990 });

            var result = _repo.Get(null, null, "id").ToList();

            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }

        [Fact]
        public void Get_SortBy_Name_Descending()
        {
            _repo.Add(new Customer { Name = "Alpha", Email = "a@mail.com", BirthYear = 2000 });
            _repo.Add(new Customer { Name = "Charlie", Email = "c@mail.com", BirthYear = 1990 });
            _repo.Add(new Customer { Name = "Bravo", Email = "b@mail.com", BirthYear = 1980 });

            var result = _repo.Get(null, null, "name_desc").ToList();

            Assert.Equal("Charlie", result[0].Name);
            Assert.Equal("Bravo", result[1].Name);
            Assert.Equal("Alpha", result[2].Name);
        }

        [Fact]
        public void Get_SortBy_BirthYear_Ascending()
        {
            _repo.Add(new Customer { Name = "C", Email = "c@mail.com", BirthYear = 2010 });
            _repo.Add(new Customer { Name = "A", Email = "a@mail.com", BirthYear = 1990 });
            _repo.Add(new Customer { Name = "B", Email = "b@mail.com", BirthYear = 2000 });

            var result = _repo.Get(null, null, "birthyear").ToList();

            Assert.Equal(1990, result[0].BirthYear);
            Assert.Equal(2000, result[1].BirthYear);
            Assert.Equal(2010, result[2].BirthYear);
        }

        // ---------------------------------------------------------
        // DATABASE CONNECTION TEST
        // ---------------------------------------------------------

        [Fact]
        public void DatabaseConnection_Works()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseSqlServer("Server=CODE-PC\\SQLEXPRESS;Database=CustomerDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            var context = new CustomerDbContext(options);

            context.Database.OpenConnection();
            context.Database.CloseConnection();
        }
    }
}