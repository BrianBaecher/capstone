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
		Console.WriteLine(json);
		var response = await _httpClient.PostAsJsonAsync($"{URI}/login", loginUser);

		if (response.IsSuccessStatusCode)
		{
			var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

			// TODO: store loginResponse info or token here (e.g., in local storage)
			Console.WriteLine($"User {loginResponse?.User.Username} logged in");
			return loginResponse;
		}
		Console.WriteLine("login failure");
		Console.WriteLine(response.ReasonPhrase);
		Console.WriteLine(username + ":" + password);
		return null;
	}

	public async Task<LoginResponse?> RegisterAsync(RegistrationInfo info)
	{
		var request = await _httpClient.PostAsJsonAsync($"{URI}/register", info);
		if (request.IsSuccessStatusCode)
		{
			Console.WriteLine("Registration success code");
			var loginResponse = await request.Content.ReadFromJsonAsync<LoginResponse>();

			return loginResponse;
		}
		Console.WriteLine("Registration fail");
		return null;
	}


}