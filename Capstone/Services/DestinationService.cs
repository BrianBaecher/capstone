using Capstone.Shared.Models;
using System.Net.Http.Json;

public class DestinationService
{
	private readonly HttpClient _httpClient;

	public DestinationService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Destination[]> GetDestinationsAsync()
	{
		return await _httpClient.GetFromJsonAsync<Destination[]>("api/destinations") ?? [];
	}
}