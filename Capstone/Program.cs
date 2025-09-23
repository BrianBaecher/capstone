using Capstone;
using Capstone.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7035/") });

// adding services for API communications
builder.Services.AddScoped<DestinationService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddScoped<TestimonialService>();
builder.Services.AddScoped<TravelPackageService>();

await builder.Build().RunAsync();
