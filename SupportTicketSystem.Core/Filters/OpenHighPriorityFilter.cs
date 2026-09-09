using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Interfaces;

namespace SupportTicketSystem.Core.Filters
{
    public class OpenHighPriorityFilter : IFilterTickets
    {
        public bool Matches(Ticket ticket)
        {
            return ticket.status == StatusValues.open && ticket.priority == PriorityValues.high;
        }
    }
}
