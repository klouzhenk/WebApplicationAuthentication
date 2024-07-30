using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using WebApplicationAuthentication.Class;
using WebApplicationAuthentication;


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
        var user = _context.Users.SingleOrDefault(u => u.Username == request.Username && u.Password == request.Password);

        if (user != null)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("XA5dpjm3TcXLloBvCtplCKI8cy9e75ubuZK+d8zlfLNbyJTbsRsDcOyyQ3grsE4j"));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim("Name", request.Username),
                new Claim("Password", request.Password),
                new Claim(ClaimTypes.Role, user.Role)

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

