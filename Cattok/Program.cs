using Blazored.Modal;
using CatTok.Services.IServices;
using CatTok.Services;
using Blazored.LocalStorage;
using CatTok.Handlers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CatTok;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();

var keyVaultUrl = builder.Configuration["KeyVault:VaultUri"];
var credential = new DefaultAzureCredential();

builder.Services.AddSingleton(x => new SecretClient(new Uri(keyVaultUrl), credential));

builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddHttpClient("CatTokAPI")
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ServerUrl"] ?? ""))
                .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddScoped<AuthenticationState>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();

builder.Services.AddAuthorizationCore();

await using var host = builder.Build();

var authService = host.Services.GetRequiredService<IAuthenticationService>();
await authService.InitializeAsync();

await host.RunAsync();
