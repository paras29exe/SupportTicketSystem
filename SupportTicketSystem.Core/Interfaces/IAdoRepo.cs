using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces
{
    public interface IAdoRepo
    {
        Task<IEnumerable<ResponseTicketDto>> GetAllTicketsAsync();
        Task<IEnumerable<ResponseTicketDto>> GetTicketByCustomerIdAsync(int customerId);
        Task<int> UpdateTicketStatusAsync(int ticketId, StatusValues newStatus);
    }
}
