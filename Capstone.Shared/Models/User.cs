namespace Capstone.Shared.Models
{
	public class User
	{
		public string? Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;

		public User Clone()
		{
			User other = new();
			other.Id = Id;
			other.Username = Username;
			other.Email = Email;
			other.Role = Role;
			return other;
		}
	}
}
