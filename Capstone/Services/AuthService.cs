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

	public async Task<User?> LoginAsync(string username, string password)
	{
		//TODO: hash
		var loginUser = new User() { Username = username, Password = password };

		var json = JsonSerializer.Serialize(loginUser);
		Console.WriteLine(json);
		var response = await _httpClient.PostAsJsonAsync($"{URI}/login", loginUser);

		if (response.IsSuccessStatusCode)
		{
			var user = await response.Content.ReadFromJsonAsync<User>();

			// TODO: store user info or token here (e.g., in local storage)
			Console.WriteLine($"User {user?.Username} logged in");
			return user;
		}
		Console.WriteLine("login failure");
		Console.WriteLine(response.ReasonPhrase);
		Console.WriteLine(username + ":" + password);
		return null;
	}


}