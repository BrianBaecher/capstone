namespace Capstone.Shared.Models
{
	public class LoginResponse
	{
		public string? Token { get; set; }
		public User? User { get; set; }
		public string? ResponseMessage { get; set; }
		public bool Success { get; set; }
	}

	public class TokenUserResponse
	{
		public string Token { get; set; }
		public User User { get; set; }
	}
}
