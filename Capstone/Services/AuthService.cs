using Capstone.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Capstone.Services;
public class AuthService
{
	private readonly HttpClient _httpClient;
	const string URI = "api/auth";

	public AuthService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<LoginResponse?> LoginAsync(string username, string password)
	{
		//TODO: hash
		var loginUser = new LoginRequest() { Username = username, Password = password };

		var json = JsonSerializer.Serialize(loginUser);
		var response = await _httpClient.PostAsJsonAsync($"{URI}/login", loginUser);

		var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

		return loginResponse;
	}

	public async Task<LoginResponse?> RegisterAsync(RegistrationInfo info)
	{
		var request = await _httpClient.PostAsJsonAsync($"{URI}/register", info);
		var loginResponse = await request.Content.ReadFromJsonAsync<LoginResponse>();

		return loginResponse;
	}

	public async Task<string> ResetPasswordRequestAsync(ResetPasswordInfo info)
	{
		var res = await _httpClient.PostAsJsonAsync($"{URI}/reset-password", info);

		return await res.Content.ReadAsStringAsync();
	}
}