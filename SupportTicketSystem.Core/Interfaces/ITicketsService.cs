using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces
{
    public interface ITicketsService
    {
        Task<PaginatedResponse<ResponseTicketDto>> getAllTicketsAsync(string? title = null, StatusValues? status = null, PriorityValues? priority = null, int? page = 1, int? pageSize = null);
        Task<ResponseTicketDto?> getTicketByIdAsync(int id);
        Task<ResponseTicketDto> createTicketAsync(CreateTicketDto ticket);
        Task<ResponseTicketDto> updateTicketAsync(int id, UpdateTicketDto updateTicketDto);

        Task<IEnumerable<ResponseTicketDto>> getTicketsByCustomerIdAsync(int customerId);
        Task<bool> updateTicketStatusAsync(int ticketId, StatusValues newStatus);

    }
}
