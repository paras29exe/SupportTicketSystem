using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Responses;
using SupportTicketSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Interfaces.ITicket;

namespace SupportTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController(ILogger<TicketController> _logger, ITicketsService _service) : ControllerBase
    {
        private readonly ILogger<TicketController> logger = _logger;
        private readonly ITicketsService service = _service;

        [HttpGet]
        public async Task<IActionResult> getTickets([FromQuery] string? title = null, [FromQuery] StatusValues? status = null, [FromQuery] PriorityValues? priority = null, [FromQuery] int? page = 1, [FromQuery] int? pageSize = null)
        {
            PaginatedResponse<ResponseTicketDto> data = await service.getAllTicketsAsync(title, status, priority, page, pageSize);

            ApiResponse<PaginatedResponse<ResponseTicketDto>> res = new(200, "Tickets fetched successfully", data);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getTicketById(int id)
        {
            ResponseTicketDto? data = await service.getTicketByIdAsync(id);

            if (data == null)
            {
                return NotFound(new ApiResponse(404, $"Ticket with Id: {id} not found"));
            }

            ApiResponse<DataWrapper<ResponseTicketDto>> res = new(200, "Ticket fetched successfully", new DataWrapper<ResponseTicketDto>(data));

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> createTicket([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException(400, "Validation failed", ModelState, ModelState);
            }

            ResponseTicketDto data = await service.createTicketAsync(dto);

            ApiResponse<DataWrapper<ResponseTicketDto>> res = new(201, "Ticket created successfully", new DataWrapper<ResponseTicketDto>(data));

            return Created("Ticket created successfully", res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateTicket(int id, [FromBody] UpdateTicketDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException(400, "Validation failed", ModelState, ModelState);
            }

            ResponseTicketDto data = await service.updateTicketAsync(id, dto);

            ApiResponse<DataWrapper<ResponseTicketDto>> res = new(200, "Ticket updated successfully", new(data));

            return Ok(res);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> getTicketsByCustomer(int customerId)
        {
            IEnumerable<ResponseTicketDto> data = await service.getTicketsByCustomerIdAsync(customerId);

            ApiResponse<DataWrapper<IEnumerable<ResponseTicketDto>>> res = new(200, "Tickets fetched successfully", new DataWrapper<IEnumerable<ResponseTicketDto>>(data));

            return Ok(res);
        }

        public class UpdateStatusDto { public StatusValues status { get; set; } }

        [HttpPatch("{ticketId}/status")]
        public async Task<IActionResult> updateStatus([FromRoute] int ticketId, [FromBody] UpdateStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException(400, "Validation failed", ModelState);
            }

            bool resOk = await service.updateTicketStatusAsync(ticketId, dto.status);

            return Ok(new ApiResponse(200, "Ticket status updated successfully"));
        }

        [HttpPost("assign/{ticketId}/agent/{agentId}")]
        public async Task<IActionResult> assignAgent([FromRoute] int ticketId, [FromRoute] int agentId)
        {
            ResponseTicketDto data = await service.assignAgentToTicketAsync(ticketId, agentId);

            ApiResponse<DataWrapper<ResponseTicketDto>> res = new(200, "Agent assigned successfully", new DataWrapper<ResponseTicketDto>(data));

            return Ok(res);
        }
    }
}
