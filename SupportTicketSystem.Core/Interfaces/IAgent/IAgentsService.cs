using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces.IAgent
{
    public interface IAgentsService
    {
        Task<PaginatedResponse<ResponseAgentDto>> getAllAgentsAsync(string? name = null, string? email = null, int? page = 1, int? pageSize = null);
        Task<ResponseAgentDto> getAgentByIdAsync(int id);
        Task<ResponseAgentDto> createAgentAsync(CreateAgentDto agent);
    }
}
