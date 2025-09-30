namespace Capstone.Shared.Models
{
	public class User
	{
		public string? Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty; // TODO: hash passwords
		public string Email { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
	}
}
