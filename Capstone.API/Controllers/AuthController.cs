using Capstone.API.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Authorization;
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
			return Unauthorized(
				new LoginResponse()
				{
					Success = false,
					ResponseMessage = $"User '{request.Username} not found'"
				}
				);

		// compare password
		if (!IsPasswordValid(request.Password, userRecord.Password))
		{
			return Unauthorized(
				new LoginResponse()
				{
					Success = false,
					ResponseMessage = $"Incorrect password"
				}
				);
		}

		LoginResponse loginResponse = new LoginResponse()
		{
			Token = GenerateJwtToken(userRecord),
			User = userRecord.GetSharedModel(),
			Success = true,
			ResponseMessage = "Login successful"
		};

		return Ok(loginResponse);
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegistrationInfo registrationInfo)
	{
		// check username unique
		var existing = await _users.Find(u => u.Username == registrationInfo.Username).FirstOrDefaultAsync();

		if (existing != null)
		{
			var failRes = new LoginResponse()
			{
				Success = false,
				ResponseMessage = $"The username '{registrationInfo.Username}' is not available."
			};
			return Conflict(failRes);
		}

		// create user in db (only non-admins can register thru site?)
		var user = new User_DB()
		{
			Id = null, // mongo will assign one
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
				User = user.GetSharedModel(),
				Success = true,
				ResponseMessage = "Registration Successful!"
			};
			return Ok(response);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			return BadRequest(ex.ToString());
		}
	}

	[HttpPost("reset-password")]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordInfo info)
	{
		// not actually going to implement this, but just respond with the user's email address, where we'd hypothetically send the reset procedure link...

		// using filter builder from mongo driver...
		var filterBuilder = Builders<User_DB>.Filter;
		var filter = filterBuilder.Empty;

		if (info.UseEmail && !string.IsNullOrWhiteSpace(info.Email))
		{
			//'&=' in the context of MongoDB filter used to combine filter conditions.
			filter &= filterBuilder.Eq(user => user.Email, info.Email);
		}
		else if (!string.IsNullOrWhiteSpace(info.Username))
		{
			filter &= filterBuilder.Eq(user => user.Username, info.Username);
		}
		else
		{
			return BadRequest();
		}

		var record = await _users.Find(filter).FirstOrDefaultAsync();

		if (record != null)
		{
			return Ok($"Sent reset instructions to {info.Email}");
		}
		return BadRequest($"We could not find an account associated with the provided username or email");
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
			// NOTE/HEADACHE: The JWT "sub" claim is mapped to ClaimTypes.NameIdentifier for compatibility with .NET identity APIs.
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

	private ClaimsPrincipal? ValidateJwtToken(string jwtStr)
	{
		var jwtSettings = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build()
			.GetSection("Jwt");

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwtSettings["Issuer"],
			ValidateAudience = true,
			ValidAudience = jwtSettings["Audience"],
			ValidateLifetime = true,
			IssuerSigningKey = key,
			ValidateIssuerSigningKey = true,
		};

		var handler = new JwtSecurityTokenHandler();
		try
		{
			var principal = handler.ValidateToken(jwtStr, tokenValidationParameters, out var validatedToken);
			return principal;
		}
		catch (Exception ex)
		{
			{
				Console.WriteLine($"JWT validation failed: {ex.Message}");
				return null;
			}
		}
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

	[Authorize(Roles = "admin")]
	[HttpGet("users")]
	public async Task<IActionResult> GetUsers()
	{
		var user_DBs = await _users.Find(_ => true).ToListAsync();
		var users = user_DBs.Select(u => u.GetSharedModel()).ToList();

		return Ok(users);
	}

	//[Authorize(Roles = "admin")]
	//[HttpPost("users")]
	//public async Task<IActionResult> CreateUser([FromBody] User user)
	//{
	//	var res = await _users.InsertOneAsync(user);
	//}

	[Authorize(Roles = "admin")]
	[HttpDelete("users")]
	public async Task<IActionResult> DeleteUser([FromQuery] string id)
	{
		var user = await _users.FindAsync(x => x.Id == id);

		if (user == null) return NotFound("No such user");

		var del = _users.DeleteOne(x => x.Id == id);

		return NoContent();
	}

	[Authorize(Roles = "admin")]
	[HttpPatch("users")]
	public async Task<IActionResult> PatchUser([FromBody] User updatedUser)
	{
		var filter = Builders<User_DB>.Filter.Eq(u => u.Id, updatedUser.Id);
		var update = Builders<User_DB>.Update
			.Set(u => u.Username, updatedUser.Username)
			.Set(u => u.Email, updatedUser.Email)
			.Set(u => u.Role, updatedUser.Role);

		var options = new FindOneAndUpdateOptions<User_DB>
		{
			ReturnDocument = ReturnDocument.After
		};

		var updatedDbUser = await _users.FindOneAndUpdateAsync(filter, update, options);

		if (updatedDbUser == null)
			return NotFound("User not found");

		return Accepted(updatedDbUser.GetSharedModel());
	}

	[HttpGet("tkn")]
	public async Task<IActionResult> GetUserFromToken()
	{
		var authHeader = Request.Headers.Authorization.FirstOrDefault();

		if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return Unauthorized();

		var jwt = authHeader.Substring("Bearer ".Length).Trim();

		var pricip = ValidateJwtToken(jwt);

		if (pricip == null) return Unauthorized();

		var userId = pricip.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? pricip.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (!string.IsNullOrEmpty(userId))
		{
			var dbUser = await _users.Find((x) => x.Id == userId).FirstOrDefaultAsync();

			if (dbUser == null) return NotFound();

			return Ok(new TokenUserResponse
			{
				Token = GenerateJwtToken(dbUser),
				User = dbUser.GetSharedModel()
			});
		}

		return NotFound();
	}
}