using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Infrastructure.Context;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Helper
{
    public class HelperRepo(AppDbContext _context) : IHelperRepo
    {
        private readonly AppDbContext context = _context;

        public async Task<bool> doesTicketExistsAsync(int id)
        {
            return await context.tickets.AnyAsync(t => t.id == id);
        }

        public async Task<bool> isTicketClosed(int id)
        {
            var ticket = await context.tickets.FirstOrDefaultAsync(t => t.id == id);
            if (ticket == null) return false;
            return ticket.status == StatusValues.closed;
        }

        public async Task<bool> doesAgentExistsAsync(int id)
        {
            return await context.agents.AnyAsync(a => a.id == id);
        }

        public async Task<bool> doesCustomerExistsAsync(int id)
        {
            return await context.customers.AnyAsync(c => c.id == id);
        }

        public async Task<bool> doesCustomerDetailsExistsAsync(string? email, string? phone)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone)) return false;

            return await context.customers.AnyAsync(c =>
                (!string.IsNullOrEmpty(email) && c.email == email) ||
                (!string.IsNullOrEmpty(phone) && c.phone == phone)
            );
        }

        public async Task<bool> doesAgentEmailExistsAsync(string email)
        {
            return await context.agents.AnyAsync(a => a.email == email);
        }
    }
}
