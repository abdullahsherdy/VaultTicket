using API.Data;
using API.Models;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
namespace API.Helpers;
public class TicketService : ITicketService
{
    private readonly AppDbContext _context;
    private readonly AESEncryptionService _aes;

    public TicketService(AppDbContext context, AESEncryptionService aes)
    {
        _context = context;
        _aes = aes;
    }

    public async Task<TicketDto> CreateTicketAsync(TicketCreateDto dto)
    {
        var booking = await _context.Bookings.FindAsync(dto.BookingId);
        if (booking == null) throw new Exception("Booking not found");

        var payload = $"{booking.Id}|{booking.EventId}|{booking.UserId}|{DateTime.UtcNow}";
        var encryptedQr = _aes.Encrypt(payload);

        // Generate QR image from encrypted QR string
        var qrImageBase64 = QRCodeHelper.GenerateQrCodeBase64(encryptedQr);

        var evnt = await _context.Events.FindAsync(booking.EventId);

        var ticket = new Ticket
        {
            BookingId = booking.Id,
            QrCode = qrImageBase64, // ✅ Store the actual QR code image (Base64)
            Status = "Active",
            ActiveUntil = evnt.Date
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return new TicketDto
        {
            Id = ticket.Id,
            BookingId = ticket.BookingId,
            QrCode = ticket.QrCode,
            Status = ticket.Status,
            ActiveUntil = ticket.ActiveUntil
        };
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Select(t => new TicketDto
            {
                Id = t.Id,
                BookingId = t.BookingId,
                QrCode = t.QrCode,
                Status = t.Status,
                ActiveUntil = t.ActiveUntil   
            }).ToListAsync();
    }

    public async Task<TicketDto> GetTicketByIdAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return null;

        return new TicketDto
        {
            Id = ticket.Id,
            BookingId = ticket.BookingId,
            QrCode = ticket.QrCode,
            Status = ticket.Status,
            ActiveUntil = ticket.ActiveUntil
        };
    }
   
    public async Task<bool> ValidateQrCodeAsync(string encryptedQr)
    {
        try
        {
            var decrypted = _aes.Decrypt(encryptedQr);
            var parts = decrypted.Split('|');

            if (parts.Length < 4) return false;
            var bookingId = int.Parse(parts[0]);
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.BookingId == bookingId);

            return ticket != null && ticket.QrCode == encryptedQr;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateTicketStatusAsync(int ticketId, string status)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return false;

        ticket.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
