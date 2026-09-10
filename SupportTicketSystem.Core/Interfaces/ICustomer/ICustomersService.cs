using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces.ICustomer
{
    public interface ICustomersService
    {
        Task<PaginatedResponse<ResponseCustomerDto>> getAllCustomersAsync(string? name = null, string? email = null, string? phone = null, int? page = 1, int? pageSize = null);
        Task<ResponseCustomerDto> getCustomerByIdAsync(int id);
        Task<ResponseCustomerDto> createCustomerAsync(CreateCustomerDto customer);
        Task<ResponseCustomerDto> updateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);
        Task<bool> deleteCustomerWithNoOpenTicketsAsync(int id);
    }
}
