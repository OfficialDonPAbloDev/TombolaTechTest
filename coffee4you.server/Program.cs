using Coffee4You.Server.Data;
using Coffee4You.Server.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    db.Database.Migrate();
    var seedPath = Path.Combine(env.ContentRootPath, "Data", "Seed", "AllTheBeans.json");
    await JsonSeeder.SeedAsync(db, seedPath);
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Coffee4You server running");

app.Run();
