using Microsoft.Extensions.Logging;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Services
{
    public class TicketsService(IEfCoreTicketsRepo _repo, IAdoRepo _adoRepo, IHelperRepo _helper, ILogger<TicketsService> _logger) : ITicketsService
    {
        private readonly IEfCoreTicketsRepo repo = _repo;
        private readonly IAdoRepo adoRepo = _adoRepo;
        private readonly IHelperRepo helper = _helper;
        private readonly ILogger<TicketsService> logger = _logger;

        public async Task<PaginatedResponse<ResponseTicketDto>> getAllTicketsAsync(string? title = null, StatusValues? status = null, PriorityValues? priority = null, int? page = 1, int? pageSize = null)
        {
            var result = await repo.getAllTicketsAsync(title, status, priority, page, pageSize);

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
                createdAt = t.createdAt
            });

            return new PaginatedResponse<ResponseTicketDto>(mapped, result.pagination);
        }

        public async Task<ResponseTicketDto?> getTicketByIdAsync(int id)
        {
            var t = await repo.getTicketByIdAsync(id);
            if (t == null) return null;

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
                createdAt = t.createdAt
            };
        }

        public async Task<ResponseTicketDto> createTicketAsync(CreateTicketDto ticketDto)
        {
            // validate customer
            if (!await helper.doesCustomerExistAsync(ticketDto.customerId))
            {
                throw new AppException(400, "Invalid customer id", null, null);
            }

            Ticket t = new Ticket()
            {
                title = ticketDto.title,
                description = ticketDto.description,
                priority = ticketDto.priority,
                status = ticketDto.status,
                customerId = ticketDto.customerId,
                agentId = ticketDto.agentId
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
                createdAt = created.createdAt
            };
        }

        public async Task<ResponseTicketDto> updateTicketAsync(int id, UpdateTicketDto updateTicketDto)
        {
            if (updateTicketDto.customerId.HasValue && !await helper.doesCustomerExistAsync(updateTicketDto.customerId.Value))
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
                createdAt = updated.createdAt
            };
        }

        public async Task<IEnumerable<ResponseTicketDto>> getTicketsByCustomerIdAsync(int customerId)
        {
            if (!await helper.doesCustomerExistAsync(customerId))
            {
                throw new AppException(400, "Invalid customer id");
            }
            IEnumerable<ResponseTicketDto> tickets = await adoRepo.getTicketsByCustomerIdAsync(customerId);
            return tickets;
        }

        public async Task<bool> updateTicketStatusAsync(int ticketId, StatusValues newStatus)
        {
            if (!await helper.doesTicketExistAsync(ticketId))
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
