using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Interfaces.IAgent;

namespace SupportTicketSystem.Infrastructure.Services
{
    public class AgentsService(IEfCoreAgentsRepo _repo, IHelperRepo _helper, IConfiguration configuration) : IAgentsService
    {
        private readonly IEfCoreAgentsRepo repo = _repo;
        private readonly IHelperRepo helper = _helper;

        private readonly int defaultPageSize = int.TryParse(configuration["Pagination:DefaultPageSize"], out int p)
            ? p
            : throw new AppException(500, "Pagination:DefaultPageSize in appsettings.json must be convertible to int");

        private readonly int maxPageSize = int.TryParse(configuration["Pagination:MaxPageSize"], out int mp)
            ? mp
            : throw new AppException(500, "Pagination:MaxPageSize in appsettings.json must be convertible to int");


        public async Task<PaginatedResponse<ResponseAgentDto>> getAllAgentsAsync(string? name = null, string? email = null, int? page = 1, int? pageSize = null)
        {
            int pageValue = page ?? 1;
            if (pageValue < 1) throw new AppException(400, "Page must be greater than or equal to 1");

            int pageSizeValue = pageSize ?? defaultPageSize;
            if (pageSizeValue < 1) throw new AppException(400, "pageSize must be greater than or equal to 1");

            // clamp pageSize to maxPageSize
            pageSizeValue = Math.Min(pageSizeValue, maxPageSize);

            var result = await repo.getAllAgentsAsync(pageValue, pageSizeValue, maxPageSize, name, email);

            var mapped = result.data.Select(a => new ResponseAgentDto()
            {
                id = a.id,
                name = a.name,
                email = a.email,
                department = a.department,
                isActive = a.isActive,
                createdAt = a.createdAt
            });

            return new PaginatedResponse<ResponseAgentDto>(mapped, result.pagination);
        }

        public async Task<ResponseAgentDto> getAgentByIdAsync(int id)
        {
            Agent? a = await repo.getAgentByIdAsync(id) ?? throw new AppException(404, $"Agent with Id: {id} does not exist");

            return new ResponseAgentDto()
            {
                id = a.id,
                name = a.name,
                email = a.email,
                department = a.department,
                isActive = a.isActive,
                createdAt = a.createdAt
            };
        }

        public async Task<ResponseAgentDto> createAgentAsync(CreateAgentDto agentDto)
        {
            // optional: ensure unique email
            // not enforcing here, let DB/upper layer handle duplicates
            if(await helper.doesAgentEmailExistsAsync(agentDto.email))
            {
                throw new AppException(400, $"Agent with email: {agentDto.email} already exists");
            }

            Agent a = new Agent()
            {
                name = agentDto.name,
                email = agentDto.email,
                department = agentDto.department
            };

            var created = await repo.createAgentAsync(a);

            return new ResponseAgentDto()
            {
                id = created.id,
                name = created.name,
                email = created.email,
                department = created.department,
                isActive = created.isActive,
                createdAt = created.createdAt
            };
        }
    }
}
