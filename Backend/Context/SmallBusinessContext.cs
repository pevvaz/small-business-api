using Microsoft.EntityFrameworkCore;

public class SmallBusinessContext : DbContext
{
    public DbSet<ContextModels.UserContextModel> Users { get; set; }
    public DbSet<ContextModels.UserDataContextModel> Admins { get; set; }
    public DbSet<ContextModels.UserDataContextModel> Employees { get; set; }
    public DbSet<ContextModels.UserDataContextModel> Clients { get; set; }
    public DbSet<ContextModels.ServiceContextModel> Services { get; set; }
    public DbSet<ContextModels.AppointmentContextModel> Appointments { get; set; }
    public DbSet<ContextModels.RefreshTokenContextModel> RefreshTokens { get; set; }

    public SmallBusinessContext(DbContextOptions<SmallBusinessContext> options) : base(options) { }
}