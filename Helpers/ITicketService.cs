using API.DTOs;
namespace API.Helpers;
public interface ITicketService
{
    Task<TicketDto> CreateTicketAsync(TicketCreateDto dto);
    Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
    Task<TicketDto> GetTicketByIdAsync(int id);
    Task<bool> ValidateQrCodeAsync(string encryptedQr);
    Task<bool> UpdateTicketStatusAsync(int ticketId, string status);
}
