using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuctionHouseWeb.Data;
using AuctionHouseWeb.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = AuctionHouseWeb.Models.LoginRequest;
using RefreshRequest = AuctionHouseWeb.Models.RefreshRequest;
using RegisterRequest = AuctionHouseWeb.Models.RegisterRequest;

namespace AuctionHouseWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;
    
    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }
    
    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            return BadRequest("Username already exists");

        var user = new User
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        
        return Ok(new { message = "User created successfully" });
    }
    
    // Hjælper metode
    private string HashPassword(string password)
    {
        byte[] salt = Encoding.UTF8.GetBytes("STATIC_SALT_FOR_EXAM");

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));
        
        return hashed;
    }

    private string GenerateJwtToken(User user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("username", user.UserName),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!)),
            signingCredentials: creds
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string HashRefreshToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    
    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Find user ved email eller username
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == request.Identifier || u.UserName == request.Identifier
        );

        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Forkert email eller password");
        
        // Generer JWT access token
        var accessToken = GenerateJwtToken(user);

        // Generer refresh token
        var refreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = HashRefreshToken(refreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            accessToken,
            refreshToken
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var hashedToken = HashRefreshToken(request.RefreshToken);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == hashedToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        // Generer ny access token
        var newAccessToken = GenerateJwtToken(user);

        // Generer også ny refresh token
        var newRefreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = HashRefreshToken(newRefreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }

}