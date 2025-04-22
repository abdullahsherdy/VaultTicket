namespace API.DTOs;
public class TicketDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string QrCode { get; set; } 
    public string Status { get; set; } 
    public DateTime ActiveUntil { get; set; }
}
