using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using API.Models.DTO;
using API.Models;
using API.Entities;
using API.Infrastructure;
using API.Models.Helpers;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly UserDbContext _context;
	private const string JwtKey = "XA5dpjm3TcXLloBvCtplCKI8cy9e75ubuZK+d8zlfLNbyJTbsRsDcOyyQ3grsE4j"; // key for signing token

	public AuthController(UserDbContext context)
	{
		_context = context;
	}

	[HttpPost("login")]
	public IActionResult Login([FromBody] LoginRequest request)
	{
		// check if password or username is not empty
		if (request == null ||
			string.IsNullOrWhiteSpace(request.Username) ||
			string.IsNullOrWhiteSpace(request.Password))
		{
			throw new CustomException("Username or password is empty. Please assign them.");
		}

		var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);
		
		// check if we have user with this name
		if (user == null) { throw new CustomException("User with this name wasn't found out."); }

		// check password
		if (user.Password != PasswordHelper.HashPassword(request.Password, user.Salt))
			throw new CustomException("Password is not correct for user with this name.");

		var (credentials, claims) = GetCredentialsAndClaims(user);

		// JWT generation
		var securityToken = new JwtSecurityToken(
			issuer: "https://172.19.100.148:7267/swagger/index.html",
			audience: "https://localhost:7147/",
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: credentials,
			claims: claims);

		var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
		// return access (JWT) and refresh token
		return Ok(new JwtResponse { Token = tokenString, RefreshToken = user.RefreshToken });
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request)
	{
		if (request == null ||
			string.IsNullOrWhiteSpace(request.Username) ||
			string.IsNullOrWhiteSpace(request.Password))
		{
			throw new CustomException("Username or password is empty. Please assign them.");
		}

		var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
		if (existingUser != null) { throw new CustomException("User with this name already exists."); }

		// salt generating and password hashing
		var salt = PasswordHelper.GenerateSalt();
		var hashedPassword = PasswordHelper.HashPassword(request.Password, salt);

		var user = new User
		{
			Username = request.Username,
			Password = hashedPassword,
			Salt = salt,
			Role = request.Role,
			IdTown = 1,
			RefreshToken = GenerateRefreshToken(),
			RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
		};

		try
		{
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
		}
		catch (Exception ex) { throw new CustomException("Adding user to DB was failed."); }

		return Ok(new { message = "User registered successfully" });
	}

	[HttpDelete("delete-self")]
	public IActionResult DeleteSelf([FromBody] RegisterRequest request)
	{
		var username = User.Identity.Name;

		if (string.IsNullOrEmpty(username))
			return Unauthorized(new { message = "User is not authenticated" });

		var user = _context.Users.SingleOrDefault(u => u.Username == username);

		if (user == null)
			return NotFound(new { message = "User not found" });

		_context.Users.Remove(user);
		_context.SaveChanges();
		return NoContent(); // return code 204 - No Content
	}

	[HttpPost("refresh-token")]
	public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
	{
		var user = _context.Users.SingleOrDefault(u => u.RefreshToken == request.RefreshToken);

		if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
			return Unauthorized(new { message = "Invalid refresh token" });

		// new JWT token generation and token refresh
		var newJwtToken = GenerateJwtToken(user);
		var newRefreshToken = GenerateRefreshToken();

		// updating the refresh token and its expiration date
		user.RefreshToken = newRefreshToken;
		user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
		_context.SaveChanges();

		return Ok(new JwtResponse { Token = newJwtToken, RefreshToken = newRefreshToken });
	}

	private string GenerateJwtToken(User user)
	{
		var (credentials, claims) = GetCredentialsAndClaims(user);

		var securityToken = new JwtSecurityToken(
			issuer: "https://localhost:7267/",
			audience: "https://localhost:7147/",
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: credentials,
			claims: claims);

		return new JwtSecurityTokenHandler().WriteToken(securityToken);
	}

	private (SigningCredentials credentials, List<Claim> claims) GetCredentialsAndClaims(User user)
	{
		var key = Encoding.UTF8.GetBytes(JwtKey);
		if (key.Length < 16) // check if size has proper size
			throw new CustomException("Invalid key size");
		var symmetricSecurityKey = new SymmetricSecurityKey(key);
		var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Role, user.Role),
			new("IdTown", user.IdTown.ToString())
		};

		return (credentials, claims);
	}

	private static string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}

public class RefreshTokenRequest
{
	public string RefreshToken { get; set; }
}

public class JwtResponse
{
	public string Token { get; set; }
	public string RefreshToken { get; set; }
}