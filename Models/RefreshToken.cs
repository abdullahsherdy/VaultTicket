// Models/RefreshToken.cs
using API.Models;
using System.ComponentModel.DataAnnotations;

public class RefreshToken
{
    public int Id { get; set; }
    [Required]
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
    // Navigation
    public int UserId { get; set; }
    public User User { get; set; }
}
