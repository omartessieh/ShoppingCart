using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop.Client;
using Shop.Client.Services;
using Blazored.SessionStorage;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7123/") });

builder.Services.AddScoped<IClientPanelService, ClientPanelService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddBlazoredSessionStorage();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
