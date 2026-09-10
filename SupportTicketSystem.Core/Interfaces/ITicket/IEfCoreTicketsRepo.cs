using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces.ITicket
{
    public interface IEfCoreTicketsRepo
    {
        Task<PaginatedResponse<Ticket>> getAllTicketsAsync(int page, int pageSize, int maxPageSize, string? title = null, StatusValues? status = null, PriorityValues? priority = null);
        Task<Ticket?> getTicketByIdAsync(int id);
        Task<Ticket> createTicketAsync(Ticket ticket);
        Task<Ticket> updateTicketAsync(int id, UpdateTicketDto updateTicketDto);
    }
}
