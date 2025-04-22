using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Data;
using API.Helpers;
using API.DTOs;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITicketService _ticketService;

    public BookingController(AppDbContext context, ITicketService ticketService)
    {
        _context = context;
        _ticketService = ticketService;
    }

    // Book an event
    [Authorize]
    [HttpPost("{eventId}")]
    public async Task<IActionResult> BookEvent(int eventId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (await _context.Bookings.AnyAsync(b => b.EventId == eventId && b.UserId == userId))
            return BadRequest("You already booked this event.");

        var evnt = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        if (evnt == null || evnt.AvailableSeats <= 0 || evnt.Date.Date < DateTime.UtcNow.Date)
            return BadRequest("Can't place your transaction.");

        var booking = new Booking
        {
            EventId = eventId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Generate ticket
        var ticketDto = await _ticketService.CreateTicketAsync(new TicketCreateDto { BookingId = booking.Id });

        var result = new
        {
            Message = "Booking successful.",
            BookingId = booking.Id,
            Ticket = ticketDto
        };

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> Cancel(int bookingId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var booking = await _context.Bookings.FindAsync(bookingId);

        if (booking == null || booking.UserId != userId)
            return NotFound("Booking not found or access denied.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return Ok("Booking cancelled.");
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null)
            return BadRequest("User not found.");

        var userId = int.Parse(userIdString);

        var bookings = await _context.Bookings
            .Include(b => b.Event)
            .Where(b => b.UserId == userId)
            .ToListAsync();

        var bookingsDTO = bookings.Select(b => new
        {
            b.Id,
            EventTitle = b.Event.Title,
            b.CreatedAt
        });

        return Ok(bookingsDTO);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Event)
            .Include(b => b.User)
            .ToListAsync();

        var bookingsDTO = bookings.Select(b => new
        {
            b.Id,
            EventTitle = b.Event.Title,
            UserName = b.User.Username,
            b.CreatedAt
        });

        return Ok(bookingsDTO);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{bookingId}")]
    public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] Booking updated)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null)
            return NotFound("Booking not found.");

        booking.EventId = updated.EventId;
        booking.CreatedAt = updated.CreatedAt;

        await _context.SaveChangesAsync();

        return Ok("Booking updated.");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("admin-delete/{id}")]
    public async Task<IActionResult> DeleteByAdmin(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound("Booking not found.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return Ok("Booking deleted by Admin.");
    }
}
