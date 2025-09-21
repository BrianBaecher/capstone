using Capstone.Shared.Models;
using System.Net.Http.Json;

public class TestimonialService
{
	private readonly HttpClient _httpClient;
	const string URI = "api/testimonials";

	public TestimonialService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<Testimonial>> GetAllAsync()
	{
		return await _httpClient.GetFromJsonAsync<List<Testimonial>>(URI) ?? [];
	}

	public async Task<Testimonial?> GetByIdAsync(string id)
	{
		return await _httpClient.GetFromJsonAsync<Testimonial>($"{URI}/{id}");
	}

	public async Task<Testimonial?> CreateAsync(Testimonial testimonial)
	{
		var response = await _httpClient.PostAsJsonAsync(URI, testimonial);
		return await response.Content.ReadFromJsonAsync<Testimonial>();
	}

	public async Task<bool> UpdateAsync(string id, Testimonial testimonial)
	{
		var response = await _httpClient.PutAsJsonAsync($"{URI}/{id}", testimonial);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteAsync(string id)
	{
		var response = await _httpClient.DeleteAsync($"{URI}/{id}");
		return response.IsSuccessStatusCode;
	}
}
