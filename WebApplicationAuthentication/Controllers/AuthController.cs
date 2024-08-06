using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

using WebApplicationAuthentication.Entities;
using WebApplicationAuthentication.Models;
using WebApplicationAuthentication.Models.DTO;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApplicationAuthentication.Models.Helpres;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserDbContext _context;

    public AuthController(UserDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);

        if (user != null && user.Password == PasswordHelper.HashPassword(request.Password, user.Salt))
        {
            var key = Encoding.UTF8.GetBytes("XA5dpjm3TcXLloBvCtplCKI8cy9e75ubuZK+d8zlfLNbyJTbsRsDcOyyQ3grsE4j");
            if (key.Length < 16) // Переконайтеся, що ключ має правильний розмір
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Invalid key size" });
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("Password", user.Password),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("IdTown", user.IdTown.ToString()),
        };

            var securityToken = new JwtSecurityToken(
                issuer: "https://localhost:7267/",
                audience: "https://localhost:7147/",
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials,
                claims: claims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return Ok(new JwtResponse { Token = tokenString });
        }

        return Unauthorized();
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Invalid request" });
        }

        var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

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

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }
  
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return Unauthorized(new { message = "Invalid refresh token" });
        }

        var newJwtToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        _context.SaveChanges();

        return Ok(new JwtResponse { Token = newJwtToken, RefreshToken = newRefreshToken });
    }

    private string GenerateJwtToken(User user)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("XA5dpjm3TcXLloBvCtplCKI8cy9e75ubuZK+d8zlfLNbyJTbsRsDcOyyQ3grsE4j"));
        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("Password", user.Password),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("IdTown", user.IdTown.ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: "https://localhost:7267/",
            audience: "https://localhost:7147/",
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials,
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
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
