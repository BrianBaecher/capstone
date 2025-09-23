using Capstone.Shared.Models;
using System.Net.Http.Json;

namespace Capstone.Services
{
	public class TravelPackageService
	{
		private readonly HttpClient _httpClient;
		const string URI = "api/TravelPackages";

		public TravelPackageService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<TravelPackage>> GetAllAsync()
		{
			return await _httpClient.GetFromJsonAsync<List<TravelPackage>>(URI) ?? [];
		}

		public async Task<TravelPackage?> GetByIdAsync(string id)
		{
			return await _httpClient.GetFromJsonAsync<TravelPackage>($"{URI}/{id}");
		}

		public async Task<TravelPackage?> CreateAsync(TravelPackage package)
		{
			var response = await _httpClient.PostAsJsonAsync(URI, package);
			return await response.Content.ReadFromJsonAsync<TravelPackage>();
		}

		public async Task<bool> UpdateAsync(string id, TravelPackage package)
		{
			var response = await _httpClient.PutAsJsonAsync($"{URI}/{id}", package);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var response = await _httpClient.DeleteAsync($"{URI}/{id}");
			return response.IsSuccessStatusCode;
		}
	}
}