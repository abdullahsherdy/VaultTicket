using System.Text.Json.Serialization;

namespace API.Models;

public class Ticket
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string QrCode { get; set; } // Encrypted QR Data
    public string Status { get; set; } // Active, Cancelled, Scanned
    public DateTime ActiveUntil { get; set; } // Until the end of event related to booking 

    [JsonIgnore]
    public Booking Booking { get; set; }
}
