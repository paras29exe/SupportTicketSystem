using SupportTicketSystem.Core.Enums;

namespace SupportTicketSystem.Core.Interfaces
{
    public interface IHelperRepo {
        Task<bool> doesTicketExistAsync(int id);
        Task<bool> isTicketClosed(int id);

        Task<bool> doesAgentExistAsync(int id);
        Task<bool> doesCustomerExistAsync(int id);

        Task<bool> doesCustomerDetailsExistAsync(string? email, string? phone);
    }
}
