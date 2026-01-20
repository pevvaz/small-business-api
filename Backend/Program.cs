using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5050");

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SmallBusinessContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});
builder.Services.AddAuthentication("main_scheme").AddJwtBearer("main_scheme", opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Settings:Secret"]!)),

        ValidAudiences = new[] { "admin", "employee", "client" },

        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<SmallBusinessContext>()!.Database.Migrate();
}

app.Run();


// NEED TO ADD A DEFAULT EXEPTION HANDLER