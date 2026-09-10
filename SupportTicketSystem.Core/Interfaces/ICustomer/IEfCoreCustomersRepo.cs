using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Models;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Interfaces.ICustomer
{
    public interface IEfCoreCustomersRepo
    {
        Task<PaginatedResponse<Customer>> getAllCustomersAsync(int page, int pageSize, int maxPageSize, string? name = null, string? email = null, string? phone = null);
        Task<Customer?> getCustomerByIdAsync(int id);
        Task<Customer> createCustomerAsync(Customer customer);
        Task<Customer> updateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);
        Task<bool> deleteCustomerWithNoOpenTicketsAsync(int id);
    }
}
