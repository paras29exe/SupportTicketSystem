using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Responses;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Interfaces.IAgent;

namespace SupportTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("api/agents")]
    public class AgentController(ILogger<AgentController> _logger, IAgentsService _service) : ControllerBase
    {
        private readonly ILogger<AgentController> logger = _logger;
        private readonly IAgentsService service = _service;

        [HttpGet]
        public async Task<IActionResult> getAgents([FromQuery] string? name = null, [FromQuery] string? email = null, [FromQuery] int? page = 1, [FromQuery] int? pageSize = null)
        {
            PaginatedResponse<ResponseAgentDto> data = await service.getAllAgentsAsync(name, email, page, pageSize);

            ApiResponse<PaginatedResponse<ResponseAgentDto>> res = new(200, "Agents fetched successfully", data);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getAgentById(int id)
        {
            ResponseAgentDto? data = await service.getAgentByIdAsync(id);

            if (data == null) return NotFound(new ApiResponse(404, $"Agent with Id: {id} not found"));

            ApiResponse<DataWrapper<ResponseAgentDto>> res = new(200, "Agent fetched successfully", new DataWrapper<ResponseAgentDto>(data));

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> createAgent([FromBody] CreateAgentDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException(400, "Validation failed", ModelState, "Look the terminal/Console for failed validation fields");
            }

            ResponseAgentDto data = await service.createAgentAsync(dto);

            ApiResponse<DataWrapper<ResponseAgentDto>> res = new(201, "Agent created successfully", new DataWrapper<ResponseAgentDto>(data));

            return Created("Agent created successfully", res);
        }
    }
}
