using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Dtos
{
    public class UpdateCustomerDto
    {
        public string? name { get; set; } = null;
        public string? email { get; set; } = null;
        public string? phone { get; set; } = null;
    }
}
