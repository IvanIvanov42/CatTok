using CatTok.Components;
using Blazored.Modal;
using CatTok.Services.IServices;
using CatTok.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredModal();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.secret.json", optional: true)
    .Build();

builder.Services.AddHttpClient("CatTokAPI", client => client.BaseAddress = new Uri(configuration["CatTokAPIUri"]));

builder.Services.AddScoped<IInstagramService, InstagramService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();