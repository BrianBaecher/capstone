namespace Capstone.Shared.Models
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; } // TODO: hash passwords
		public string Email { get; set; }
		public string Role { get; set; }
	}
}
