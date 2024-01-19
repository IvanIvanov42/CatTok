using Expatery_API.Controllers;
using Expatery_API.Models;
using Expatery_API.Services;
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
    .AddJsonFile("appsettings.secret.json", optional: true) // Add this line
    .Build();

string instagramAccessToken = configuration["InstagramAccessToken"];
string azureSqlConnection = configuration.GetConnectionString("Billie-Jean-TV-SQL");

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