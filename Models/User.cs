// Models/User.cs
namespace API.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    [Required]
    public string Role { get; set; } = "Guest"; //"User" or "Admin"
    [JsonIgnore] // to avoid json cycle 
    public List<RefreshToken> RefreshTokens { get; set; }
    [JsonIgnore]
    public List<Booking> Bookings { get; set; }
}
