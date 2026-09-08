using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Models
{
    public class ApiError
    {
        public bool success { get; } = false;
        public int status { get; set; }
        public string message { get; set; } = string.Empty;
        public object? details { get; set; }

    }
}
