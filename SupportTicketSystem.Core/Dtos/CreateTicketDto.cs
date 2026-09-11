using SupportTicketSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Dtos
{
    public class CreateTicketDto
    {
        [Required(ErrorMessage = "Ticket title is required.")]
        public string title { get; set; } = string.Empty;

        public string? description { get; set; }

        [Required(ErrorMessage = "Ticket priority is required.")]
        public PriorityValues priority { get; set; } = PriorityValues.low;

        [Required(ErrorMessage = "Customer ID is required.")]
        public int customerId { get; set; }
    }
}
