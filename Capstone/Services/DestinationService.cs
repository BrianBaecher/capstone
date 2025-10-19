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

	public async Task<bool> DeleteDestinationAsync(Destination d)
	{
		string id = d.Id ?? string.Empty;
		if (string.IsNullOrWhiteSpace(id))
			throw new ArgumentException("id is null or whitespace", nameof(id));

		if (id.Length != 24)
			Console.WriteLine($"Warning: id '{id}' length is {id.Length}; controller expects 24-char Mongo ObjectId.");

		string endpoint = $"{URI}/{id}";
		var res = await _httpClient.DeleteAsync(endpoint);

		if (!res.IsSuccessStatusCode)
		{
			var body = await res.Content.ReadAsStringAsync();
			Console.WriteLine($"DELETE {endpoint} failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");
		}

		return res.IsSuccessStatusCode;
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