using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using API.Models;
using API.DTOs;
using API.Helpers;
using API.Data;
using Microsoft.AspNetCore.Identity.Data; // JwtService, PasswordHasher, etc.

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordHasher _passwordHasher; // Changed to interface

    public AuthController(AppDbContext context, JwtService jwtService, PasswordHasher passwordHasher)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] API.DTOs.RegisterRequest request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (userExists)
            return BadRequest("Username already taken.");

        var hashed = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Username = request.Username,
            PasswordHash = hashed,
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] API.DTOs.LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = _jwtService.GenerateToken(user);
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            token,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] API.DTOs.RefreshRequest request)
    {
        var tokenEntity = await _context.RefreshTokens.Include(r => r.User)
                            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (tokenEntity == null || tokenEntity.Expires < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token.");

        var newToken = _jwtService.GenerateToken(tokenEntity.User);
        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = tokenEntity.User.Id
        };

        // Invalidate old token (optional: delete it)
        _context.RefreshTokens.Remove(tokenEntity);
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            token = newToken,
            refreshToken = newRefreshToken.Token
        });
    }
}
