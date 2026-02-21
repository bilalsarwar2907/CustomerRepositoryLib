using Microsoft.EntityFrameworkCore;
using CustomerRepositoryLib.Models;

namespace CustomerRepositoryLib.Repositories
{
    // This class represents your database.
    // EF Core uses DbContext to connect to SQL Server and track changes.
    public class CustomerDbContext : DbContext
    {
        // The constructor receives configuration options (like connection string).
        // These options are passed in from your test or your application.
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        // This property represents the Customers table in your SQL database.
        // EF Core will map the Customer model to the Customers table.
        public DbSet<Customer> Customers { get; set; }
    }
}