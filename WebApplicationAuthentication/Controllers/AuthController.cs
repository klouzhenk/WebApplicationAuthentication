using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    // Хардкодим ім'я користувача та пароль
    private const string HardcodedUsername = "admin";
    private const string HardcodedPassword = "admin";

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Перевірка хардкодених значень
        if (request.Username == HardcodedUsername && request.Password == HardcodedPassword)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-secure-and-long-secret-key-should-be-at-least-32-bytes"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://example.com",
                audience: "https://myapi.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new JwtResponse { Token = tokenString });
        }

        return Unauthorized();
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class JwtResponse
{
    public string Token { get; set; }
}