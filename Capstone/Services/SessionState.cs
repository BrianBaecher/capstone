using Blazored.LocalStorage;
using Capstone.Shared.Models;

namespace Capstone.Services;
public class SessionState
{
	private readonly ILocalStorageService _localStorage;
	public User? CurrentUser { get; private set; }
	public event Action? OnChange;

	public SessionState(ILocalStorageService lclStorage)
	{
		_localStorage = lclStorage;
	}

	public async Task SetUserAsync(User? user, string? token)
	{
		CurrentUser = user;
		if (token != null)
		{
			await _localStorage.SetItemAsync("authToken", token);
		}
		OnChange?.Invoke();
	}

	public void SetUser(User? user)
	{
		CurrentUser = user;
		OnChange?.Invoke();
	}

	public void Logout()
	{
		CurrentUser = null;
		OnChange?.Invoke();
	}
}