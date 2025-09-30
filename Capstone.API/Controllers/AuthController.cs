using Capstone.API.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IMongoCollection<User_DB> _users;

	public AuthController(IMongoClient mongoClient)
	{
		var database = mongoClient.GetDatabase("Capstone");
		_users = database.GetCollection<User_DB>("users");
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] User login)
	{
		Console.WriteLine($"FROM BACK: {login.Username}:{login.Password}");

		var user = await _users.Find(u => u.Username == login.Username && u.Password == login.Password).FirstOrDefaultAsync();
		if (user == null)
			return Unauthorized();

		// TODO: Generate JWT or session token here
		return Ok(user.GetSharedModel());
	}

	[HttpGet]
	public async Task<List<User>> GetUsers()
	{
		var records = await _users.Find(_ => true).ToListAsync();

		List<User> users = new List<User>();
		foreach (var record in records)
		{
			users.Add(record.GetSharedModel());
		}
		return users;
	}
}