using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Interfaces.IAgent;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Infrastructure.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Data
{
    public class EfCoreAgentsRepo(AppDbContext _context) : IEfCoreAgentsRepo
    {
        private readonly AppDbContext context = _context;

        public async Task<PaginatedResponse<Agent>> getAllAgentsAsync(int page, int pageSize, int maxPageSize, string? name = null, string? email = null)
        {
            IQueryable<Agent> query = context.agents.AsNoTracking();

            if (!string.IsNullOrEmpty(name)) query = query.Where(a => a.name.Contains(name));
            if (!string.IsNullOrEmpty(email)) query = query.Where(a => a.email.Contains(email));

            var totalCount = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(totalCount / (float)pageSize);

            var data = await query
                .OrderBy(a => a.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = new Core.Models.Pagination(page, totalPages, totalCount, pageSize, maxPageSize);

            return new PaginatedResponse<Agent>(data, pagination);
        }

        public async Task<Agent?> getAgentByIdAsync(int id)
        {
            return await context.agents.FirstOrDefaultAsync(a => a.id == id);
        }

        public async Task<Agent> createAgentAsync(Agent agent)
        {
            await context.agents.AddAsync(agent);
            await context.SaveChangesAsync();
            return agent;
        }
    }
}
