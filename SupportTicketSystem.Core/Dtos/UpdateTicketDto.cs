using SupportTicketSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Dtos
{
    public class UpdateTicketDto 
    {
        [Required(ErrorMessage = "Ticket title is required.")]
        public string title { get; set; } = string.Empty;

        public string? description { get; set; }

        public PriorityValues? priority { get; set; } = null;

        public StatusValues? status { get; set; } = null;

        public int? customerId { get; set; } = null;

        public int? agentId { get; set; }
    }
}
