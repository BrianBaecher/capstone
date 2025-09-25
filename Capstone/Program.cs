using Capstone;
using Capstone.Models;
using Capstone.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

{
	// temporary HttpClient to read appsettings.json and pass to ConfigHelper
	// added scope to dispose of tempClient (Builder.RunAsync() never returns until the application closes, so without this added scope the tempClient is never disposed of...)
	using var tempClient = new HttpClient()
	{
		BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
	};

	var settings = await tempClient.GetStringAsync("appsettings.json");

	AppConfigHelper.OnStart(settings);
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppConfigHelper.ApiBaseUrl) });

// adding services for API communications
builder.Services.AddScoped<DestinationService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddScoped<TestimonialService>();
builder.Services.AddScoped<TravelPackageService>();
builder.Services.AddScoped<ImageService>();

await builder.Build().RunAsync();
