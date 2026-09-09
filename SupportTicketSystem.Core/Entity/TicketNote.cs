using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Base;

namespace SupportTicketSystem.Core.Entity
{
    public class TicketNote : BaseEntity
    {
        public int ticketId { get; set; }
        public string noteText { get; set; } = string.Empty;

        public Ticket? ticket { get; set; }
    }
}
