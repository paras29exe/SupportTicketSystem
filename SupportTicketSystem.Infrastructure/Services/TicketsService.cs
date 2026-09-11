using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Interfaces.ITicket;

namespace SupportTicketSystem.Infrastructure.Services
{
    public class TicketsService(IEfCoreTicketsRepo _repo, IAdoRepo _adoRepo, IHelperRepo _helper, IConfiguration configuration) : ITicketsService
    {
        private readonly IEfCoreTicketsRepo repo = _repo;
        private readonly IAdoRepo adoRepo = _adoRepo;
        private readonly IHelperRepo helper = _helper;

        private readonly int defaultPageSize = int.TryParse(configuration["Pagination:DefaultPageSize"], out int p)
            ? p
            : throw new AppException(500, "Pagination:DefaultPageSize in appsettings.json must be convertible to int");

        private readonly int maxPageSize = int.TryParse(configuration["Pagination:MaxPageSize"], out int mp)
            ? mp
            : throw new AppException(500, "Pagination:MaxPageSize in appsettings.json must be convertible to int");

        public async Task<PaginatedResponse<ResponseTicketDto>> getAllTicketsAsync(string? title = null, StatusValues? status = null, PriorityValues? priority = null, int? page = 1, int? pageSize = null)
        {
            int pageValue = page ?? 1;
            if (pageValue < 1) throw new AppException(400, "Page must be greater than or equal to 1");

            int pageSizeValue = pageSize ?? defaultPageSize;
            if (pageSizeValue < 1) throw new AppException(400, "pageSize must be greater than or equal to 1");

            // clamp pageSize to maxPageSize
            pageSizeValue = Math.Min(pageSizeValue, maxPageSize);

            var result = await repo.getAllTicketsAsync(pageValue, pageSizeValue, maxPageSize, title, status, priority);

            var mapped = result.data.Select(t => new ResponseTicketDto()
            {
                id = t.id,
                title = t.title,
                description = t.description,
                priority = t.priority,
                status = t.status,
                customerId = t.customerId,
                customerName = t.customer?.name,
                agentId = t.agentId,
                agentName = t.agent?.name,
                createdAt = t.createdAt,
                closedAt = t.closedAt
            });

            return new PaginatedResponse<ResponseTicketDto>(mapped, result.pagination);
        }

        public async Task<ResponseTicketDto> assignAgentToTicketAsync(int ticketId, int agentId)
        {
            // validate ticket exists
            if (!await helper.doesTicketExistsAsync(ticketId))
            {
                throw new AppException(400, "Invalid ticket id");
            }

            // validate agent exists
            if (!await helper.doesAgentExistsAsync(agentId))
            {
                throw new AppException(400, "Invalid agent id");
            }

            UpdateTicketDto update = new UpdateTicketDto { title="", agentId = agentId };

            var updated = await repo.updateTicketAsync(ticketId, update);

            return new ResponseTicketDto()
            {
                id = updated.id,
                title = updated.title,
                description = updated.description,
                priority = updated.priority,
                status = updated.status,
                customerId = updated.customerId,
                customerName = updated.customer?.name,
                agentId = updated.agentId,
                agentName = updated.agent?.name,
                createdAt = updated.createdAt,
                closedAt = updated.closedAt
            };
        }

        public async Task<ResponseTicketDto> getTicketByIdAsync(int id)
        {
            var t = await repo.getTicketByIdAsync(id) ?? throw new AppException(404, "Invalid Ticket id");

            return new ResponseTicketDto()
            {
                id = t.id,
                title = t.title,
                description = t.description,
                priority = t.priority,
                status = t.status,
                customerId = t.customerId,
                customerName = t.customer?.name,
                agentId = t.agentId,
                agentName = t.agent?.name,
                createdAt = t.createdAt,
                closedAt = t.closedAt
            };
        }

        public async Task<ResponseTicketDto> createTicketAsync(CreateTicketDto ticketDto)
        {
            // validate customer
            if (!await helper.doesCustomerExistsAsync(ticketDto.customerId))
            {
                throw new AppException(400, "Invalid customer id", null, null);
            }

            Ticket t = new Ticket()
            {
                title = ticketDto.title,
                description = ticketDto.description,
                priority = ticketDto.priority,
                // status is not accepted from caller; default to open
                status = StatusValues.open,
                customerId = ticketDto.customerId,
                agentId = null
            };

            var created = await repo.createTicketAsync(t);

            return new ResponseTicketDto()
            {
                id = created.id,
                title = created.title,
                description = created.description,
                priority = created.priority,
                status = created.status,
                customerId = created.customerId,
                customerName = created.customer?.name,
                agentId = created.agentId,
                agentName = created.agent?.name,
                createdAt = created.createdAt,
                closedAt = created.closedAt
            };
        }

        public async Task<ResponseTicketDto> updateTicketAsync(int id, UpdateTicketDto updateTicketDto)
        {
            if (updateTicketDto.customerId.HasValue && !await helper.doesCustomerExistsAsync(updateTicketDto.customerId.Value))
            {
                throw new AppException(400, "Invalid customer id", null, null);
            }

            var updated = await repo.updateTicketAsync(id, updateTicketDto);

            return new ResponseTicketDto()
            {
                id = updated.id,
                title = updated.title,
                description = updated.description,
                priority = updated.priority,
                status = updated.status,
                customerId = updated.customerId,
                customerName = updated.customer?.name,
                agentId = updated.agentId,
                agentName = updated.agent?.name,
                createdAt = updated.createdAt,
                closedAt = updated.closedAt
            };
        }

        public async Task<IEnumerable<ResponseTicketDto>> getTicketsByCustomerIdAsync(int customerId)
        {
            if (!await helper.doesCustomerExistsAsync(customerId))
            {
                throw new AppException(400, "Invalid customer id");
            }
            IEnumerable<ResponseTicketDto> tickets = await adoRepo.getTicketsByCustomerIdAsync(customerId);
            return tickets;
        }

        public async Task<bool> updateTicketStatusAsync(int ticketId, StatusValues newStatus)
        {
            if (!await helper.doesTicketExistsAsync(ticketId))
            {
                throw new AppException(400, "Invalid ticket id");
            }

            int res = await adoRepo.updateTicketStatusAsync(ticketId, newStatus);

            if (res == 409)
            {
                throw new AppException(409, "Ticket status is already Closed. So can't update");
            }

            return true;
        }
    }
}
