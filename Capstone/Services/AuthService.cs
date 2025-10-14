using Blazored.LocalStorage;
using Capstone.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Capstone.Services;
public class AuthService
{
	private readonly HttpClient _httpClient;
	private readonly ILocalStorageService _localStorage;
	private readonly SessionState _sessionState;

	const string URI = "api/auth";

	public AuthService(HttpClient httpClient, ILocalStorageService localStorage, SessionState sessionState)
	{
		_httpClient = httpClient;
		_localStorage = localStorage;
		_sessionState = sessionState;
	}

	public async Task<LoginResponse?> LoginAsync(string username, string password)
	{
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

	public async Task<List<User>> GetUsersAsync()
	{
		var token = await _localStorage.GetItemAsync<string>("authToken");
		if (!string.IsNullOrEmpty(token))
		{
			_httpClient.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
		}

		return await _httpClient.GetFromJsonAsync<List<User>>($"{URI}/users") ?? new();
	}

	public async Task<bool> DeleteUserAsync(string userID)
	{
		var response = await _httpClient.DeleteAsync($"{URI}/users?id={userID}");

		return response.IsSuccessStatusCode;
	}

	public async Task<User?> UpdateUserAsync(User editedUser)
	{
		var response = await _httpClient.PatchAsJsonAsync<User>($"{URI}/users", editedUser);

		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<User>();
		}
		return null;
	}

	public async Task TryAutoLogin()
	{
		var token = await _localStorage.GetItemAsync<string>("authToken");
		if (!string.IsNullOrEmpty(token))
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetFromJsonAsync<TokenUserResponse>("api/auth/tkn");

				if (response != null)
				{
					await _localStorage.SetItemAsync<string>("authToken", response.Token);
					_sessionState.SetUser(response.User);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine($"auto-login failed: {ex.Message}");
			}
		}
	}
}