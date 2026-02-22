using CustomerRepositoryLib.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. Add DbContext (SQL Server)
// ---------------------------------------------------------
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(
        "Server=CODE-PC\\SQLEXPRESS;Database=CustomerDB;Trusted_Connection=True;TrustServerCertificate=True;"
    )
);

// ---------------------------------------------------------
// 2. Register Repository (choose LIST or DATABASE)
// ---------------------------------------------------------

bool useDatabase = true;   // ← switch between List and Database

if (useDatabase)
{
    builder.Services.AddScoped<ICustomerRepository, CustomerRepositoryDatabase>();
}
else
{
    builder.Services.AddScoped<ICustomerRepository, CustomerRepositoryList>();
}

// ---------------------------------------------------------
// 3. Add Controllers + Swagger
// ---------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------------------------------------------------------
// 4. Configure HTTP pipeline
// ---------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();