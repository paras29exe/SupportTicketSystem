using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Entity;

namespace SupportTicketSystem.Core.Interfaces
{
    public interface FilterTickets
    {
        bool Matches(Ticket ticket);
    }
}
