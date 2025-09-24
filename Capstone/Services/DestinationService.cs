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
		var response = await _httpClient.PutAsJsonAsync($"{URI}/{id}", destination);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> CreateDestinationAsync(Destination d)
	{
		var res = await _httpClient.PostAsJsonAsync<Destination>(URI, d);
		return res.IsSuccessStatusCode;
	}
}