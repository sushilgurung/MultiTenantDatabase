

using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces.Services
{
    public interface ITenantUserManagementService
    {
        Task<bool> RegisterTenantAsync(string email, string password, int tenantId);
        Task<(SignInResult signInResult, TenantUser user)> AuntenticationAsync(string email, string password, int tenantId);
    }
}
