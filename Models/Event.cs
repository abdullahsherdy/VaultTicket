// Models/Event.cs
namespace API.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// mapping to a database table 
/// EF Runtime, Convert each class to a database table 
/// POCO classes
/// constraints, data types, 
/// Annotations ( name, data type, length, required, etc.)
/// Identity, by default -> int (identity (1,1) )
/// data
/// page -> Events 
/// api/all -> Bookings 
public class Event
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    /// not null 
    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime Date { get; set; }

    public int AvailableSeats { get; set; }

    /// to avoid json cycle 
    [JsonIgnore] /// attributes, tags, DataAnnotations
    public List<Booking> Bookings { get; set; } // navigation property, map forigen key primary key relation 
    // one - many relation
}
