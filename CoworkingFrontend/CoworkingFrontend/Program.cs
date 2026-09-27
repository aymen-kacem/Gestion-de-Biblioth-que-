using CoworkingBlazor;
using CoworkingBlazor.Services;
using CoworkingFrontend;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with BaseAddress (do NOT use HttpClientHandler in Blazor WebAssembly)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7141/") // Update to your API HTTPS URL
});

// Configure default JSON serializer options
var options = new System.Text.Json.JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
    Converters = { new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase) }
};

// Store options for use in services (if needed)
builder.Services.AddScoped<System.Text.Json.JsonSerializerOptions>(_ => options);

// Register services
// AuthService requires IJSRuntime, so register via factory
builder.Services.AddScoped<IAuthService>(sp => new AuthService(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IJSRuntime>()));
builder.Services.AddScoped<IAbonnementService, AbonnementService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IEspaceService, EspaceService>();
builder.Services.AddScoped<IRessourceService, RessourceService>();

var host = builder.Build();

// Initialize authentication (restore token from localStorage and set headers)
try
{
    var auth = host.Services.GetRequiredService<IAuthService>();
    await auth.InitializeAsync();
}
catch
{
    // ignore initialization errors
}

await host.RunAsync();