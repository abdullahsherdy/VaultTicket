// Models/User.cs
namespace API.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    [Required]
    public string Role { get; set; } = "User"; //"User" or "Admin"
    public List<RefreshToken> RefreshTokens { get; set; }
    public List<Booking> Bookings { get; set; }
}
