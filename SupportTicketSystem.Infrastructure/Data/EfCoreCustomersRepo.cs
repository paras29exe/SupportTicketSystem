using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Interfaces.ICustomer;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Infrastructure.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Data
{
    public class EfCoreCustomersRepo(AppDbContext _context) : IEfCoreCustomersRepo
    {
        private readonly AppDbContext context = _context;

        public async Task<PaginatedResponse<Customer>> getAllCustomersAsync(int page, int pageSize, int maxPageSize, string? name = null, string? email = null, string? phone = null)
        {
            IQueryable<Customer> query = context.customers.AsNoTracking();

            if (!string.IsNullOrEmpty(name)) query = query.Where(c => c.name.Contains(name));
            if (!string.IsNullOrEmpty(email)) query = query.Where(c => c.email.Contains(email));
            if (!string.IsNullOrEmpty(phone)) query = query.Where(c => c.phone != null && c.phone.Contains(phone));

            var totalCount = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(totalCount / (float)pageSize);

            var data = await query
                .OrderBy(c => c.id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = new Core.Models.Pagination(page, totalPages, pageSize, maxPageSize);

            return new PaginatedResponse<Customer>(data, pagination);
        }

        public async Task<Customer?> getCustomerByIdAsync(int id)
        {
            Customer? c = await context.customers.FirstOrDefaultAsync(cu => cu.id == id);
            return c;
        }

        public async Task<Customer> createCustomerAsync(Customer customer)
        {
            await context.customers.AddAsync(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> updateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            Customer c = await context.customers.FirstAsync(x => x.id == id);

            if (updateCustomerDto.name is not null) c.name = updateCustomerDto.name;
            if (updateCustomerDto.email is not null) c.email = updateCustomerDto.email;
            if (updateCustomerDto.phone is not null) c.phone = updateCustomerDto.phone;

            await context.SaveChangesAsync();

            return c;
        }

        public async Task<bool> deleteCustomerWithNoOpenTicketsAsync(int id)
        {
            Customer c = await context.customers
                                            .Include(data => data.tickets)
                                            .FirstAsync(cu => cu.id == id)!;

            bool hasOpen = c.tickets != null && c.tickets.Any(t => t.status == Core.Enums.StatusValues.open || t.status == Core.Enums.StatusValues.inProgress);

            if (hasOpen) return false;

            context.customers.Remove(c);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
