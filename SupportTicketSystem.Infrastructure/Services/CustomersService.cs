using Microsoft.Extensions.Logging;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Core.Interfaces.ICustomer;

namespace SupportTicketSystem.Infrastructure.Services
{
    public class CustomersService(IEfCoreCustomersRepo _repo, IHelperRepo _helper, ILogger<CustomersService> _logger, IConfiguration configuration) : ICustomersService
    {
        private readonly IEfCoreCustomersRepo repo = _repo;
        private readonly IHelperRepo helper = _helper;
        private readonly ILogger<CustomersService> logger = _logger;

        private readonly int defaultPageSize = int.TryParse(configuration["Pagination:DefaultPageSize"], out int p)
            ? p
            : throw new AppException(500, "DefaultPageSize value under Key named 'Pagination' in app.settings must be convertible to int");

        private readonly int maxPageSize = int.TryParse(configuration["Pagination:MaxPageSize"], out int mp)
            ? mp
            : throw new AppException(500, "MaxPageSize value under Key named 'Pagination' in app.settings must be convertible to int");


        public async Task<PaginatedResponse<ResponseCustomerDto>> getAllCustomersAsync(string? name = null, string? email = null, string? phone = null, int? page = 1, int? pageSize = null)
        {
            int pageValue = page ?? 1;
            if (pageValue < 1) throw new AppException(400, "Page must be greater than or equal to 1");

            int pageSizeValue = pageSize ?? defaultPageSize;
            if (pageSizeValue < 1) throw new AppException(400, "pageSize must be greater than or equal to 1");

            // clamp pageSize to maxPageSize
            pageSizeValue = Math.Min(pageSizeValue, maxPageSize);

            var result = await repo.getAllCustomersAsync(pageValue, pageSizeValue, maxPageSize, name, email, phone);

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
            if (await helper.doesCustomerDetailsExistsAsync(customerDto.email, customerDto.phone))
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
            if (!string.IsNullOrEmpty(updateCustomerDto.email) && await helper.doesCustomerDetailsExistsAsync(updateCustomerDto.email, null))
            {
                throw new AppException(409, "Email already in use");
            }

            if (!string.IsNullOrEmpty(updateCustomerDto.phone) && await helper.doesCustomerDetailsExistsAsync(null, updateCustomerDto.phone))
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
            if (!await helper.doesCustomerExistsAsync(id))
            {
                throw new AppException(404, $"Customer with Id: {id} does not exist");
            }

            bool res = await repo.deleteCustomerWithNoOpenTicketsAsync(id);

            if (!res) throw new AppException(400, "Customer has open tickets and cannot be deleted");

            return true;
        }
    }
}
