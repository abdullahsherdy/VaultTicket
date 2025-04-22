namespace API.DTOs;
/// <summary>
///  only one value is needed to create a ticket, Booking id, and USERID will be taken from the booking table or from Claims 
/// </summary>
public class TicketCreateDto
{
    public int BookingId { get; set; }
}
