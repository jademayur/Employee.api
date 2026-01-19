using Microsoft.EntityFrameworkCore;

namespace Employee.api.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
       public DbSet<Employee> Employees { get; set; }
       public DbSet<Department> Departments { get; set; }
       public DbSet<Designation> Designations { get; set; }
    }
}
