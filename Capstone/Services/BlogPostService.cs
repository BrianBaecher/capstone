using Capstone.Shared.Models;
using System.Net.Http.Json;

namespace Capstone.Services
{
	public class BlogPostService
	{
		private readonly HttpClient _httpClient;
		const string URI = "api/BlogPosts";

		public BlogPostService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<BlogPost>> GetAllAsync()
		{
			return await _httpClient.GetFromJsonAsync<List<BlogPost>>(URI) ?? [];
		}

		public async Task<List<BlogPost>> GetLimited(int limit)
		{
			return await _httpClient.GetFromJsonAsync<List<BlogPost>>($"{URI}?limit={limit}") ?? [];
		}

		public async Task<BlogPost?> GetByIdAsync(string id)
		{
			return await _httpClient.GetFromJsonAsync<BlogPost>($"{URI}/{id}");
		}

		public async Task<BlogPost?> CreateAsync(BlogPost post)
		{
			var response = await _httpClient.PostAsJsonAsync(URI, post);
			return await response.Content.ReadFromJsonAsync<BlogPost>();
		}

		public async Task<bool> UpdateAsync(string id, BlogPost post)
		{
			var response = await _httpClient.PutAsJsonAsync($"{URI}/{id}", post);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var response = await _httpClient.DeleteAsync($"{URI}/{id}");
			return response.IsSuccessStatusCode;
		}
	}
}