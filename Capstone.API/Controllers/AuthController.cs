using Capstone.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IMongoCollection<User> _users;

	public AuthController(IMongoClient mongoClient)
	{
		var database = mongoClient.GetDatabase("Capstone");
		_users = database.GetCollection<User>("users");
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] User login)
	{
		var user = await _users.Find(u => u.Username == login.Username && u.Password == login.Password).FirstOrDefaultAsync();
		if (user == null)
			return Unauthorized();

		// TODO: Generate JWT or session token here
		return Ok(new { user.Username, user.Role });
	}
}