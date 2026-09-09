using SupportTicketSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Dtos
{
    public class ResponseTicketDto
    {
        [Required(ErrorMessage = "Ticket ID is required.")]
        public int id { get; set; }
        [Required(ErrorMessage = "Ticket title is required.")]
        public string title { get; set; } = string.Empty;
        public string? description { get; set; } = null;
        [Required(ErrorMessage = "Ticket priority is required.")]
        public PriorityValues priority { get; set; } = PriorityValues.low;
        [Required(ErrorMessage = "Ticket status is required.")]
        public StatusValues status { get; set; } = StatusValues.open;
        [Required(ErrorMessage = "Customer ID is required.")]
        public int customerId { get; set; }
        public string? customerName { get; set; } = null;
        public int? agentId { get; set; }
        public string? agentName { get; set; } = null;
        public DateTime createdAt { get; set; }
    }
}
