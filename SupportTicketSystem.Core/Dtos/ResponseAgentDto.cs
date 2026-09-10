using System;

namespace SupportTicketSystem.Core.Dtos
{
    public class ResponseAgentDto
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? department { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }
    }
}
