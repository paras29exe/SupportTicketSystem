using Microsoft.Extensions.Logging;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SupportTicketSystem.Infrastructure.Services
{
    public class CustomersService(IEfCoreCustomersRepo _repo, IHelperRepo _helper, ILogger<CustomersService> _logger) : ICustomersService
    {
        private readonly IEfCoreCustomersRepo repo = _repo;
        private readonly IHelperRepo helper = _helper;
        private readonly ILogger<CustomersService> logger = _logger;

        public async Task<PaginatedResponse<ResponseCustomerDto>> getAllCustomersAsync(string? name = null, string? email = null, string? phone = null, int? page = 1, int? pageSize = null)
        {
            var result = await repo.getAllCustomersAsync(name, email, phone, page, pageSize);

            var mapped = result.data.Select(c => new ResponseCustomerDto()
            {
                id = c.id,
                name = c.name,
                email = c.email,
                phone = c.phone ?? string.Empty
            });

            return new PaginatedResponse<ResponseCustomerDto>(mapped, result.pagination);
        }

        public async Task<ResponseCustomerDto?> getCustomerByIdAsync(int id)
        {
            Customer? c = await repo.getCustomerByIdAsync(id);

            if (c == null)
            {
                throw new AppException(404, $"Customer with Id: {id} does not exist");
            }

            return new ResponseCustomerDto()
            {
                id = c.id,
                name = c.name,
                email = c.email,
                phone = c.phone ?? string.Empty
            };
        }

        public async Task<ResponseCustomerDto> createCustomerAsync(CreateCustomerDto customerDto)
        {
            if (await helper.doesCustomerDetailsExistAsync(customerDto.email, customerDto.phone))
            {
                throw new AppException(409, "Customer with same email or phone already exists");
            }

            Customer c = new Customer()
            {
                name = customerDto.name,
                email = customerDto.email,
                phone = customerDto.phone
            };

            Customer created = await repo.createCustomerAsync(c);

            return new ResponseCustomerDto()
            {
                id = created.id,
                name = created.name,
                email = created.email,
                phone = created.phone ?? string.Empty
            };
        }

        public async Task<ResponseCustomerDto> updateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            // if email/phone updated ensure uniqueness
            if (!string.IsNullOrEmpty(updateCustomerDto.email) && await helper.doesCustomerDetailsExistAsync(updateCustomerDto.email, null))
            {
                throw new AppException(409, "Email already in use");
            }

            if (!string.IsNullOrEmpty(updateCustomerDto.phone) && await helper.doesCustomerDetailsExistAsync(null, updateCustomerDto.phone))
            {
                throw new AppException(409, "Phone already in use");
            }

            Customer updated = await repo.updateCustomerAsync(id, updateCustomerDto);

            return new ResponseCustomerDto()
            {
                id = updated.id,
                name = updated.name,
                email = updated.email,
                phone = updated.phone ?? string.Empty
            };
        }

        public async Task<bool> deleteCustomerWithNoOpenTicketsAsync(int id)
        {
            if (!await helper.doesCustomerExistAsync(id))
            {
                throw new AppException(404, $"Customer with Id: {id} does not exist");
            }

            bool res = await repo.deleteCustomerWithNoOpenTicketsAsync(id);

            if (!res) throw new AppException(400, "Customer has open tickets and cannot be deleted");

            return true;
        }
    }
}
