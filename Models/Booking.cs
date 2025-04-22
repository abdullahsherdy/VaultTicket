// Models/Booking.cs

using System.Text.Json.Serialization;

namespace API.Models;
/// <summary>
/// SQl Server, SQLLite 
/// </summary>
public class Booking
{
    public int Id { get; set; }
    // Default value
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation
    public int UserId { get; set; }
    public User User { get; set; }
    public int EventId { get; set; }
    [JsonIgnore]
    public Event Event { get; set; }
    [JsonIgnore]
    public Ticket Ticket { get; set; }
}
