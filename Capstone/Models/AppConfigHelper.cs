using Capstone.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using System.Text.Json;

namespace Capstone.Models
{
	public static class AppConfigHelper
	{
		public static string ApiBaseUrl { get; private set; } = string.Empty;

		public static void OnStart(string appSettingsJSON)
		{
			using var doc = JsonDocument.Parse(appSettingsJSON);
			var root = doc.RootElement;

			// get any properties used in the helper, update field in this class...
			if (root.TryGetProperty("ApiBaseUrl", out var url))
			{
				var asString = url.GetString();
				if (asString == null) throw new Exception("missing 'ApiBaseUrl' key in appsettings.json");

				// trim off last slash if it's on the url
				ApiBaseUrl = asString.Trim().TrimEnd('/');
			}
		}

		public static class ServiceManager
		{
			// stores all the types of services used in webassembly program, and the lifetime of service.
			private static readonly (Type type, ServiceLifetime lifetime)[] Services = new[]
			{
				(typeof(DestinationService), ServiceLifetime.Scoped),
				(typeof(BlogPostService), ServiceLifetime.Scoped),
				(typeof(ImageService), ServiceLifetime.Scoped),
				(typeof(TestimonialService), ServiceLifetime.Scoped),
				(typeof(TravelPackageService), ServiceLifetime.Scoped),
				(typeof(DialogService), ServiceLifetime.Scoped), // Radzen service type, not my creation.
				(typeof(AuthService), ServiceLifetime.Scoped),
				(typeof(SessionState), ServiceLifetime.Singleton),
			};

			private enum ServiceLifetime
			{
				Scoped,
				Singleton,
				Transient
			}

			public static void RegisterServices(WebAssemblyHostBuilder builder)
			{
				Dictionary<ServiceLifetime, Func<Type, IServiceCollection>> registrationDelegateDict = new()
				{
					{ ServiceLifetime.Scoped, builder.Services.AddScoped },
					{ ServiceLifetime.Singleton, builder.Services.AddSingleton },
					{ ServiceLifetime.Transient, builder.Services.AddTransient },
				};
				foreach (var service in Services)
				{
					if (registrationDelegateDict.TryGetValue(service.lifetime, out var func))
					{
						func.Invoke(service.type);
					}
				}

			}
		}
	}
}
