using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces.IAgent
{
    public interface IEfCoreAgentsRepo
    {
        Task<PaginatedResponse<Agent>> getAllAgentsAsync(int page, int pageSize, int maxPageSize, string? name = null, string? email = null);
        Task<Agent?> getAgentByIdAsync(int id);
        Task<Agent> createAgentAsync(Agent agent);
    }
}
