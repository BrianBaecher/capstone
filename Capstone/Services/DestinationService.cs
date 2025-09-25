using Capstone.Shared.Models;
using System.Net.Http.Json;

public class DestinationService
{
	private readonly HttpClient _httpClient;
	const string URI = "api/destinations";

	public DestinationService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Destination[]> GetAllAsync()
	{
		return await _httpClient.GetFromJsonAsync<Destination[]>(URI) ?? [];
	}

	public async Task<bool> UpdateByIdAsync(string id, Destination destination)
	{
		Console.WriteLine(id);

		EnsureAvifExtension(destination);

		var response = await _httpClient.PutAsJsonAsync($"{URI}/{id}", destination);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> CreateDestinationAsync(Destination d)
	{
		var res = await _httpClient.PostAsJsonAsync<Destination>(URI, d);
		return res.IsSuccessStatusCode;
	}


	private static void EnsureAvifExtension(Destination d)
	{
		// ensure destination image filename has extension (all uploaded images are converted to .avif)
		if (!Path.HasExtension(d.ImageFilename) || Path.GetExtension(d.ImageFilename) != ".avif")
		{
			d.ImageFilename = Path.ChangeExtension(d.ImageFilename, ".avif");
		}
	}
}