using Microsoft.EntityFrameworkCore;
namespace EHI_Assignment.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
             .HasIndex(e => new { e.FirstName, e.LastName, e.Email })
             .IsUnique();
            base.OnModelCreating(modelBuilder);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("EHIDB");
        }
        public static void SeedData(ApplicationDbContext context)
        {
            if (!context.Employees.Any())
            {
                var employees = new List<Employee>
            {
                new Employee
                {
                    FirstName = "Ramesh1",
                    LastName = "Nair1",
                    Email = "Ramesh.Nair@gmail.com",
                    Age = 30,
                    Address = new Address
                {
                    Street = "456 right St",
                    City = "NewTown",
                    State = "FL",
                    PostalCode = "78956"
                }
                },
                new Employee
                {
                    FirstName = "Jane1",
                    LastName = "Smith1",
                    Email = "jane.smith@gmail.com",
                    Age = 28,
                    Address = new Address
                {
                    Street = "Borivali",
                    City = "Anytown",
                    State = "CA",
                    PostalCode = "12345"
                }
            }
            };

                context.Employees.AddRange(employees);
                context.SaveChanges();
            }
        }
    }
}
