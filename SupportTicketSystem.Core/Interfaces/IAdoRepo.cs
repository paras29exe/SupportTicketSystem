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
        Task<IEnumerable<ResponseTicketDto>> getAllTicketsAsync();
        Task<IEnumerable<ResponseTicketDto>> getTicketsByCustomerIdAsync(int customerId);
        Task<int> updateTicketStatusAsync(int ticketId, StatusValues newStatus);
    }
}
