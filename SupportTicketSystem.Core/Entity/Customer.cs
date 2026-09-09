using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Base;

namespace SupportTicketSystem.Core.Entity
{
    public class Customer : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? phone { get; set; }

        public ICollection<Ticket>? tickets { get; set; }
    }
}
