using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Data;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    //  Public: Get All Events
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _context.Events.ToListAsync();
        return Ok(events);
    }

    //  Public: Get Event by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
            return NotFound("Event not found.");

        return Ok(ev);
    }

    // 🔒 Admin: Create Event
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(Event ev)
    {
        _context.Events.Add(ev);
        await _context.SaveChangesAsync();
        return Ok(ev);
    }

    // 🔒 Admin: Update Event
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Event updated)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound("Event not found.");

        ev.Title = updated.Title;
        ev.Date = updated.Date;
        ev.Description = updated.Description;

        await _context.SaveChangesAsync();
        return Ok(ev);
    }

    // 🔒 Admin: Delete Event
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound("Event not found.");

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return Ok("Deleted.");
    }
}
