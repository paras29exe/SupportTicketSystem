using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Responses;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Interfaces.ICustomer;

namespace SupportTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController(ILogger<CustomerController> _logger, ICustomersService _service) : ControllerBase
    {
        private readonly ILogger<CustomerController> logger = _logger;
        private readonly ICustomersService service = _service;

        [HttpGet]
        public async Task<IActionResult> getCustomers([FromQuery] string? name = null, [FromQuery] string? email = null, [FromQuery] string? phone = null, [FromQuery] int? page = 1, [FromQuery] int? pageSize = null)
        {
            PaginatedResponse<ResponseCustomerDto> data = await service.getAllCustomersAsync(name, email, phone, page, pageSize);

            ApiResponse<PaginatedResponse<ResponseCustomerDto>> res = new(200, "Customers fetched successfully", data);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getCustomerById(int id)
        {
            ResponseCustomerDto data = await service.getCustomerByIdAsync(id);

            ApiResponse<DataWrapper<ResponseCustomerDto>> res = new(200, "Customer fetched successfully", new DataWrapper<ResponseCustomerDto>(data));

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> createCustomer([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException(400, "Validation failed", ModelState, "Look the terminal/Console for failed validation fields");
            }

            ResponseCustomerDto data = await service.createCustomerAsync(dto);

            ApiResponse<DataWrapper<ResponseCustomerDto>> res = new(201, "Customer created successfully", new DataWrapper<ResponseCustomerDto>(data));

            return Created("Customer created successfully", res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException(400, "Validation failed", ModelState);
            }

            ResponseCustomerDto data = await service.updateCustomerAsync(id, dto);

            ApiResponse<DataWrapper<ResponseCustomerDto>> res = new(200, "Customer updated successfully", new DataWrapper<ResponseCustomerDto>(data));

            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCustomer(int id)
        {
            bool deleted = await service.deleteCustomerWithNoOpenTicketsAsync(id);

            return Ok(new ApiResponse(200, "Customer deleted successfully"));
        }
    }
}
