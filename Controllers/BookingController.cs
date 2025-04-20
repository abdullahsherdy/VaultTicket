using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Models;
using API.Data;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    // 🔒 Book an event
    [Authorize]
    [HttpPost("{eventId}")]
    public async Task<IActionResult> BookEvent(int eventId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (await _context.Bookings.AnyAsync(b => b.EventId == eventId && b.UserId == userId))
            return BadRequest("You already booked this event.");

        var booking = new Booking
        {
            EventId = eventId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok("Booking successful.");
    }


    [Authorize]
    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> Cancel(int bookingId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var booking = await _context.Bookings.FindAsync(bookingId);

        if (booking == null || booking.UserId != userId)
            return NotFound("Booking not found or access denied.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return Ok("Booking cancelled.");
    }

    // 🔒 User: Get own bookings
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var bookings = await _context.Bookings
                        .Include(b => b.Event)
                        .Where(b => b.UserId == userId)
                        .ToListAsync();

        return Ok(bookings);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _context.Bookings
                            .Include(b => b.Event)
                            .Include(b => b.User)
                            .ToListAsync();

        return Ok(bookings);
    }
}
