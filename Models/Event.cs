// Models/Event.cs
namespace API.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Event
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime Date { get; set; }

    public int AvailableSeats { get; set; }

    [JsonIgnore]
    public List<Booking> Bookings { get; set; }
}
