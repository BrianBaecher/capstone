using Capstone;
using Capstone.Models;
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

// adding services via static helper
AppConfigHelper.ServiceManager.RegisterServices(builder);

await builder.Build().RunAsync();
