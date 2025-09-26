using Capstone.Shared.Models;
using System.Net.Http.Json;

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
		var response = await _httpClient.PostAsJsonAsync(URI, loginUser);

		if (response.IsSuccessStatusCode)
		{
			var user = await response.Content.ReadFromJsonAsync<User>();

			// TODO: store user info or token here (e.g., in local storage)

			return user;
		}
		return null;
	}


}