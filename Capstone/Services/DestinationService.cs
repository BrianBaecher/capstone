using Capstone.Shared.Models;
using System.Net.Http.Json;

namespace Capstone.Services;
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

	public async Task<Destination?> GetByIdAsync(string id)
	{
		var result = await _httpClient.GetFromJsonAsync<Destination>($"{URI}/{id}");

		return result;
	}

	public async Task<bool> UpdateByIdAsync(string id, Destination destination)
	{
		//Console.WriteLine(id);

		EnsureAvifExtension(destination);

		var response = await _httpClient.PutAsJsonAsync($"{URI}/{id}", destination);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> CreateDestinationAsync(Destination d)
	{
		EnsureAvifExtension(d);

		var res = await _httpClient.PostAsJsonAsync<Destination>(URI, d);
		return res.IsSuccessStatusCode;
	}

	public async Task<bool> IsDestinationCodeUnique(string code)
	{
		string endpoint = $"{URI}/destination-code/isunique?code={code}";
		Console.WriteLine(endpoint);
		return await _httpClient.GetFromJsonAsync<bool>(endpoint);
	}

	#region validating
	private static void EnsureAvifExtension(Destination d)
	{
		// ensure destination image filename has extension (all uploaded images are converted to .avif)
		if (!Path.HasExtension(d.ImageFilename) || Path.GetExtension(d.ImageFilename) != ".avif")
		{
			d.ImageFilename = Path.ChangeExtension(d.ImageFilename, ".avif");
		}
	}
	#endregion
}