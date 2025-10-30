using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using MudBlazor.Services;
using Selmir.MudGridify.Playground;
using Selmir.MudGridify.Playground.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Localization
builder.Services.AddLocalization();

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Culture Service
builder.Services.AddScoped<ICultureService, CultureService>();

var host = builder.Build();

// The culture is already set by Blazor.start() in index.html
// We just need to ensure the current culture is properly set
var culture = CultureInfo.CurrentCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
