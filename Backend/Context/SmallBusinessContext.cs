using Microsoft.EntityFrameworkCore;

public class SmallBusinessContext : DbContext
{
    public SmallBusinessContext(DbContextOptions<SmallBusinessContext> options) : base(options) { }

    public DbSet<ContextModels.SessionContextModel> Sessions { get; set; }
    public DbSet<ContextModels.UserContextModel> Admins { get; set; }
    public DbSet<ContextModels.UserContextModel> Employees { get; set; }
    public DbSet<ContextModels.UserContextModel> Clients { get; set; }
    public DbSet<ContextModels.ServiceContextModel> Services { get; set; }
    public DbSet<ContextModels.AppointmentContextModel> Appointments { get; set; }
    public DbSet<ContextModels.RefreshTokenContextModel> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContextModels.SessionContextModel>(opt =>
        {
            opt.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContextModels.RefreshTokenContextModel>(opt =>
        {
            opt.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}