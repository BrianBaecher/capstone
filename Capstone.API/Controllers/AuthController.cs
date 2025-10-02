using Capstone.API.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		Console.WriteLine($"FROM BACK: {request.Username}:{request.Password}");

		var userRecord = await _users.Find(u => u.Username == request.Username).FirstOrDefaultAsync();
		if (userRecord == null)
			return Unauthorized();

		// compare password
		if (!IsPasswordValid(request.Password, userRecord.Password))
		{
			return Unauthorized();
		}

		LoginResponse loginResponse = new LoginResponse();

		loginResponse.Token = GenerateJwtToken(userRecord);
		loginResponse.User = userRecord.GetSharedModel();

		return Ok(loginResponse);
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


	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegistrationInfo registrationInfo)
	{
		// check username unique
		var existing = await _users.Find(u => u.Username == registrationInfo.Username).FirstOrDefaultAsync();

		if (existing != null)
		{
			return Conflict($"The username {registrationInfo.Username} is not available.");
		}

		// create user in db (only non-admins can register thru site?)
		var user = new User_DB()
		{
			Id = null, // mongo will assign one (in theory idk)
			Username = registrationInfo.Username,
			Password = HashPassword(registrationInfo.Password),
			Email = registrationInfo.Email,
			Role = "user"
		};

		try
		{
			await _users.InsertOneAsync(user);

			var response = new LoginResponse()
			{
				Token = GenerateJwtToken(user),
				User = user.GetSharedModel()
			};
			return Ok(response);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			return BadRequest(ex.ToString());
		}
	}










	private string GenerateJwtToken(User_DB user)
	{
		var jwtSettings = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build()
			.GetSection("Jwt");

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
		new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? ""),
		new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
		new Claim(ClaimTypes.Role, user.Role)
		};

		var token = new JwtSecurityToken(
			issuer: jwtSettings["Issuer"],
			audience: jwtSettings["Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private string HashPassword(string password)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(16);

		var subkey = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);

		byte[] hash = subkey.GetBytes(32);

		return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
	}

	private bool IsPasswordValid(string password, string storedHash)
	{
		var split = storedHash.Split('.');
		if (split.Length != 2) return false;

		byte[] salt = Convert.FromBase64String(split[0]);
		byte[] hash = Convert.FromBase64String(split[1]);

		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);

		var enteredHashed = pbkdf2.GetBytes(32);

		return CryptographicOperations.FixedTimeEquals(hash, enteredHashed);
	}

}