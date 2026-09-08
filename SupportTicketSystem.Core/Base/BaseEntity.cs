using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Base
{
    public abstract class BaseEntity
    {
        public int id { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
