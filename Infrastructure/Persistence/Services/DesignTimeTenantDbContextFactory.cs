using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class DesignTimeTenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();

            // Use a sample tenant connection string for generating migrations
            optionsBuilder.UseSqlServer("Your_Temporary_Tenant_Connection_String");

            return new TenantDbContext(optionsBuilder.Options, null);
        }
    }
}
