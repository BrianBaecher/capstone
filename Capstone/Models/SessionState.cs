using Capstone.Shared.Models;

namespace Capstone.Services;
public class SessionState
{
	//TODO: JWT
	public User? CurrentUser { get; private set; }

	public event Action? OnChange;

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