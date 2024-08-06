using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

using API.Entities;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using API.Models.DTO;
using API.Models;


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

            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return Ok(new JwtResponse { Token = tokenString });
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Перевірка вхідних даних
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Invalid request" });
        }

        // Перевірка чи користувач вже існує
        var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Генерація солі та хешування паролю
        var salt = PasswordHelper.GenerateSalt();
        var hashedPassword = PasswordHelper.HashPassword(request.Password, salt);

        // Створення нового користувача
        var user = new User
        {
            Username = request.Username,
            Password = hashedPassword,
            Salt = salt,
            Role = request.Role,
            IdTown = 1
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }
}

public static class PasswordHelper
{
    public static string GenerateSalt()
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    public static string HashPassword(string password, string salt)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        return hashed;
    }
}