using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Base;
using SupportTicketSystem.Core.Enums;

namespace SupportTicketSystem.Core.Entity
{
    public class Ticket : BaseEntity
    {
        public string title { get; set; } = string.Empty;
        public string? description { get; set; }
        public PriorityValues priority { get; set; } = PriorityValues.low;
        public StatusValues status { get; set; } = StatusValues.open;
        public int customerId { get; set; }
        public int? agentId { get; set; }
        public DateTime? closedAt { get; set; }

        // Navigation
        public Customer? customer { get; set; }
        public Agent? agent { get; set; }
        public ICollection<TicketNote>? notes { get; set; }
    }
}
