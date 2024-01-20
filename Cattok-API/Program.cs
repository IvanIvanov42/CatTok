using Cattok_API.Models;
using Cattok_API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<InstagramService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read the Instagram access token from the configuration file
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.secret.json", optional: true)
    .Build();

string instagramAccessToken = configuration["InstagramAccessToken"];
string azureSqlConnection = configuration.GetConnectionString("SQL-CATTOK");

builder.Services.AddDbContext<InstagramDataStorageDbContext>(options =>
    options.UseSqlServer(azureSqlConnection));

builder.Services.AddScoped<IInstagramDataStorage, DatabaseInstagramDataStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Pass the Instagram access token to the InstagramService
app.Use((context, next) =>
{
    context.Items["InstagramAccessToken"] = instagramAccessToken;
    return next();
});

app.MapControllers();

app.Run();