using SupportTicketSystem.Core.Enums;

namespace SupportTicketSystem.Core.Interfaces
{
    public interface IHelperRepo {
        Task<bool> doesTicketExistsAsync(int id);
        Task<bool> isTicketClosed(int id);

        Task<bool> doesAgentExistsAsync(int id);
        Task<bool> doesCustomerExistsAsync(int id);

        Task<bool> doesCustomerDetailsExistsAsync(string? email, string? phone);
        Task<bool> doesAgentEmailExistsAsync(string email);
    }
}
