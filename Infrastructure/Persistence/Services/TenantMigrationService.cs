using Application.Common.Provider;
using Application.Interfaces.Services;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.DataSeed.FakeDataGenerator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class TenantMigrationService(MainDbContext _mainDbContext)
    {
        public async Task MigrateAllTenantsAsync()
        {
            var tenants = await _mainDbContext.Tenants.AsNoTracking().ToListAsync();

            foreach (var tenant in tenants)
            {
                string connectionString = ConnectionStringProvider.GetConnectionString(tenant.DataSource, tenant.Catalog, tenant.UserId, tenant.Password);
                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(connectionString);  // Use tenant's connection string

                using (var tenantContext = new TenantDbContext(optionsBuilder.Options, null))
                {
                    // Apply migrations to each tenant database
                    await tenantContext.Database.MigrateAsync();
                    //  await AddAdminTenants(tenantContext, tenant);
                }

            }
        }

        //public async Task AddAdminTenants(TenantDbContext tenantDbContext, Tenants tenants)
        //{
        //    var adminGenerator = DataGenerator.GetAdminTenants(tenants.Name);
        //    var generatedTenants = adminGenerator.Generate(1);
        //    string password = "P@ssword123";

        //    foreach (var tenant in generatedTenants)
        //    {
        //        await tenantDbContext.Users.AddAsync(tenant);
        //        await _tenantLoginService.RegisterTenantAsync(tenant.Email, password, tenants.Id);
        //    }
        //}
    }
}
