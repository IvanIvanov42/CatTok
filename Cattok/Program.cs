using Blazored.Modal;
using CatTok.Services.IServices;
using CatTok.Services;
using Blazored.LocalStorage;
using CatTok.Handlers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CatTok;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();

builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddHttpClient("CatTokAPI")
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ServerUrl"] ?? ""))
                .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddScoped<AuthenticationState>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
