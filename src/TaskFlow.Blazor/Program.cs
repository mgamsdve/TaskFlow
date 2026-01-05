using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskFlow.Blazor;
using TaskFlow.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7066";
if (!apiBaseUrl.EndsWith('/'))
{
    apiBaseUrl += "/";
}

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProjectApiService>();
builder.Services.AddScoped<BoardApiService>();
builder.Services.AddScoped<TaskApiService>();

var host = builder.Build();
await host.Services.GetRequiredService<AuthService>().InitializeAsync();
await host.RunAsync();
