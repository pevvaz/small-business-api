using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5050");

builder.Services.AddControllers();

builder.Services.AddDbContext<SmallBusinessContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<SmallBusinessContext>()!.Database.Migrate();
}

app.Run();
