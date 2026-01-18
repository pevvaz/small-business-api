using Microsoft.EntityFrameworkCore;

public class SmallBusinessContext : DbContext
{
    public DbSet<EmployeeModel> Employees { get; set; }
    public DbSet<ClientModel> Clients { get; set; }
    public DbSet<ServiceModel> Services { get; set; }
    public DbSet<AppointmentModel> Appointments { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

    public SmallBusinessContext(DbContextOptions<SmallBusinessContext> options) : base(options) { }
}