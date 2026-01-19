using Microsoft.EntityFrameworkCore;

public class SmallBusinessContext : DbContext
{
    public DbSet<ContextModels.UserContextModel> Admins { get; set; }
    public DbSet<ContextModels.UserContextModel> Employees { get; set; }
    public DbSet<ContextModels.UserContextModel> Clients { get; set; }
    public DbSet<ContextModels.ServiceContextModel> Services { get; set; }
    public DbSet<ContextModels.AppointmentContextModel> Appointments { get; set; }
    public DbSet<ContextModels.RefreshTokenContextModel> RefreshTokens { get; set; }

    public SmallBusinessContext(DbContextOptions<SmallBusinessContext> options) : base(options) { }
}