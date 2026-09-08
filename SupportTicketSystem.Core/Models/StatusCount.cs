using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Enums;

namespace SupportTicketSystem.Core.Models
{
    public class StatusCount
    {
        public StatusValues status { get; set; }
        public int count { get; set; }
    }
}
