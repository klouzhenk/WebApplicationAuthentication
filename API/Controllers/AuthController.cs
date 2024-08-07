﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Models.DTO;
using API.Models;
using API.Entities;
using API.Models.Helpres;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserDbContext _context;
    private const string JwtKey = "XA5dpjm3TcXLloBvCtplCKI8cy9e75ubuZK+d8zlfLNbyJTbsRsDcOyyQ3grsE4j"; // Ключ для підпису токенів

    public AuthController(UserDbContext context)
    {
        _context = context;
    }

    // Метод для входу користувача
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Пошук користувача за ім'ям
        var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);

        // Перевірка правильності пароля
        if (user != null && user.Password == PasswordHelper.HashPassword(request.Password, user.Salt))
        {
            var key = Encoding.UTF8.GetBytes(JwtKey);
            if (key.Length < 16) // Переконатися, що ключ має правильний розмір
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Invalid key size" });
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("IdTown", user.IdTown.ToString())
            };

            // Генерація JWT токену
            var securityToken = new JwtSecurityToken(
                issuer: "https://localhost:7267/",
                audience: "https://localhost:7147/",
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials,
                claims: claims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            // Повернення токену та рефреш токену
            return Ok(new JwtResponse { Token = tokenString, RefreshToken = user.RefreshToken });
        }

        // Повернення статусу Unauthorized, якщо аутентифікація не вдалася
        return Unauthorized();
    }

    // Метод для реєстрації нового користувача
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Invalid request" });
        }

        // Перевірка чи користувач з таким ім'ям вже існує
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
            IdTown = 1,
            RefreshToken = GenerateRefreshToken(), // Генерація рефреш токену
            RefreshTokenExpiryTime = DateTime.Now.AddDays(7) // Встановлення терміну дії рефреш токену
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Повернення успішного повідомлення
        return Ok(new { message = "User registered successfully" });
    }

    [HttpDelete("delete-self")]
    public IActionResult DeleteSelf()
    {
        // Отримати ім'я користувача з контексту аутентифікації
        var username = User.Identity.Name;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new { message = "User is not authenticated" });
        }

        // Знайти користувача в базі даних за ім'ям
        var user = _context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Видалити користувача з бази даних
        _context.Users.Remove(user);
        _context.SaveChanges();

        return NoContent(); // Повертає код 204 No Content
    }

    // Метод для оновлення токену
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // Пошук користувача за рефреш токеном
        var user = _context.Users.SingleOrDefault(u => u.RefreshToken == request.RefreshToken);

        // Перевірка валідності рефреш токену
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return Unauthorized(new { message = "Invalid refresh token" });
        }

        // Генерація нового JWT токену та рефреш токену
        var newJwtToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        // Оновлення рефреш токену та його терміну дії
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        _context.SaveChanges();

        // Повернення нового JWT токену та рефреш токену
        return Ok(new JwtResponse { Token = newJwtToken, RefreshToken = newRefreshToken });
    }

    // Метод для генерації JWT токену
    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(JwtKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("IdTown", user.IdTown.ToString())
        };

        var securityToken = new JwtSecurityToken(
            issuer: "https://localhost:7267/",
            audience: "https://localhost:7147/",
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials,
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    // Метод для генерації рефреш токену
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

// Клас для запиту рефреш токену
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}

// Клас для відповіді з токенами
public class JwtResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
