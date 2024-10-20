using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITenancyManagerService
    {
        Task<bool> AddTenantDatabseAsync(string Name,
         string BusinessName,
         string Logo,
         string DataSource,
         string Catalog,
         string UserId,
         string Password);
        Task<bool> RenameDatabaseAsync(string dataSource, string userId, string password, string oldDatabaseName, string newDatabaseName);
    }
}
