using Coffee4You.Server.Data;
using Coffee4You.Server.Data.Seed;
using Coffee4You.Server.Domain.Common;
using Coffee4You.Server.Endpoints;
using Coffee4You.Server.Services;
using Microsoft.EntityFrameworkCore;

const string ClientCorsPolicy = "ClientCors";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IBeanOfTheDayService, BeanOfTheDayService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(ClientCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
builder.Services.Configure<ConfigSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    db.Database.Migrate();
    var seedPath = Path.Combine(env.ContentRootPath, "Data", "Seed", "AllTheBeans.json");
    await JsonSeeder.SeedAsync(db, seedPath);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(ClientCorsPolicy);

app.MapBeansEndpoints();
app.MapCountriesEndpoints();
app.MapColoursEndpoints();

app.MapGet("/", () => "Coffee4You server running");

app.Run();
