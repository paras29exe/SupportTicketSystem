using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Dtos
{
    public class ResponseCustomerDto
    {
        [Required(ErrorMessage = "Customer id is required.")]
        public int id { get; set; }
        [Required(ErrorMessage = "Customer name is required.")]
        public string name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Customer email is required.")]
        public string email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Customer phone is required.")]
        public string phone { get; set; } = string.Empty;
    }
}
