using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Interfaces.ITicket;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Infrastructure.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Data
{
    public class EfCoreTicketsRepo(AppDbContext _context) : IEfCoreTicketsRepo
    {
        private readonly AppDbContext context = _context;

        public async Task<PaginatedResponse<Ticket>> getAllTicketsAsync(int page, int pageSize, int maxPageSize, string? title = null, StatusValues? status = null, PriorityValues? priority = null)
        {
            IQueryable<Ticket> query = context.tickets.AsNoTracking();

            if (!string.IsNullOrEmpty(title)) query = query.Where(t => t.title.Contains(title));
            if (status.HasValue) query = query.Where(t => t.status == status.Value);
            if (priority.HasValue) query = query.Where(t => t.priority == priority.Value);

            var totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (float)pageSize);

            var data = await query
                .OrderBy(t => t.id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = new Pagination(page, totalPages, pageSize, maxPageSize);

            return new PaginatedResponse<Ticket>(data, pagination);
        }

        public async Task<Ticket?> getTicketByIdAsync(int id)
        {
            Ticket? t = await context.tickets
                .Include(ti => ti.customer)
                .Include(ti => ti.agent)
                .Include(ti => ti.notes)
                .FirstOrDefaultAsync(ti => ti.id == id);

            return t;
        }

        public async Task<Ticket> createTicketAsync(Ticket ticket)
        {
            await context.tickets.AddAsync(ticket);
            await context.SaveChangesAsync();

            return ticket;
        }

        public async Task<Ticket> updateTicketAsync(int id, UpdateTicketDto updateTicketDto)
        {
            Ticket t = await context.tickets.FirstAsync(x => x.id == id);

            if(String.IsNullOrEmpty(updateTicketDto.title) == false) t.title = updateTicketDto.title;

            if (updateTicketDto.description is not null) t.description = updateTicketDto.description;
            if (updateTicketDto.priority.HasValue) t.priority = updateTicketDto.priority.Value;
            if (updateTicketDto.status.HasValue) t.status = updateTicketDto.status.Value;
            if (updateTicketDto.customerId.HasValue) t.customerId = updateTicketDto.customerId.Value;
            if (updateTicketDto.agentId.HasValue) t.agentId = updateTicketDto.agentId;

            await context.SaveChangesAsync();

            return t;
        }
    }
}
