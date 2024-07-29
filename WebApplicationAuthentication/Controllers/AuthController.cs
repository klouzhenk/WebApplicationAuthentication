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
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("XA5dpjm3TcXLloBvCtplCKI8cy9e75ubuZK+d8zlfLNbyJTbsRsDcOyyQ3grsE4j"));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim("Name", request.Username),
                new Claim("Password", request.Password)
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