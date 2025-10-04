using System.Text.Json;

public class NytTravelStoryService : BackgroundService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IConfiguration _config;
	private readonly string _wwwrootPath;

	private readonly string _statusFilePath;

	public NytTravelStoryService(IHttpClientFactory httpClientFactory, IConfiguration config, IWebHostEnvironment env)
	{
		_httpClientFactory = httpClientFactory;
		_config = config;
		_wwwrootPath = Path.Combine(env.WebRootPath, "travelstories.json");
		_statusFilePath = Path.Combine(env.ContentRootPath, "data", "nyt_fetch_status.json");
		Directory.CreateDirectory(Path.GetDirectoryName(_statusFilePath)!);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var lastFetch = await GetLastFetchTimeAsync();
			if (lastFetch == null || (DateTime.UtcNow - lastFetch.Value) > TimeSpan.FromHours(24))
			{
				await FetchAndSaveStories();
				await SetLastFetchTimeAsync(DateTime.UtcNow);
			}
			await Task.Delay(TimeSpan.FromHours(12), stoppingToken); // check every 12 hr
		}
	}

	private async Task FetchAndSaveStories()
	{
		var apiKey = _config["NYT:ApiKey"];
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetAsync($"https://api.nytimes.com/svc/topstories/v2/travel.json?api-key={apiKey}");
		if (response.IsSuccessStatusCode)
		{
			var json = await response.Content.ReadAsStringAsync();
			await File.WriteAllTextAsync(_wwwrootPath, json);
		}
	}

	private async Task<DateTime?> GetLastFetchTimeAsync()
	{
		if (!File.Exists(_statusFilePath)) return null;
		var json = await File.ReadAllTextAsync(_statusFilePath);
		var doc = JsonDocument.Parse(json);
		if (doc.RootElement.TryGetProperty("lastFetch", out var lastFetch))
			return lastFetch.GetDateTime();
		return null;
	}

	private async Task SetLastFetchTimeAsync(DateTime time)
	{
		var json = JsonSerializer.Serialize(new { lastFetch = time });
		await File.WriteAllTextAsync(_statusFilePath, json);
	}
}