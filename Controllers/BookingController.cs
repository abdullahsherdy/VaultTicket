using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Data;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet("test")]
    public IActionResult Test() => Ok("Routing works!");

    // Book an event
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

    // User: Get own bookings
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

    //Admin: Update booking time or event
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

    //  Admin: Delete any booking
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
