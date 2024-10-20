using Application.Interfaces.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Provider
{
    public interface ITenantContextProvider
    {
        Task<ITenantDbContext> GetTenantDbContextAsync(int tenantId);
        Task<ITenantDbContext> GetTenantDbContextAsync(string tenantName);


        Task SetTenantDbContextAsync(int tenantId);
        Task SetTenantDbContextAsync(string tenantName);
        Task<string> GetConnectionStringForTenantByIdAsync(int tenantId);
        Task<string> GetConnectionStringForTenantByNameAsync(string tenantName);

    }
}
