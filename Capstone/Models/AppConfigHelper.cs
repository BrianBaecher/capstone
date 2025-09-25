using System.Text.Json;

namespace Capstone.Models
{
	public static class AppConfigHelper
	{
		public static string ApiBaseUrl { get; set; } = string.Empty;


		public static void OnStart(string appSettingsJSON)
		{
			using var doc = JsonDocument.Parse(appSettingsJSON);
			var root = doc.RootElement;

			// get any properties used in the helper, update field in this class...
			if (root.TryGetProperty("ApiBaseUrl", out var url))
			{
				var asString = url.GetString();
				if (asString == null) throw new Exception("missing api base url in appsettings.json");

				// trim off last slash if it's on the url
				ApiBaseUrl = asString.Trim().TrimEnd('/');
			}
		}
	}
}
