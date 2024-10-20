using Application.Common.Provider;
using Application.Interfaces.DbContext;
using Application.Interfaces.Provider;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Provider
{

    public class TenantContextProvider : ITenantContextProvider
    {
        private readonly ICurrentTenantService _currentTenantService;
        private readonly MainDbContext _mainDbContext;
        private readonly IMemoryCache _memoryCache;
        public TenantContextProvider(
            ICurrentTenantService currentTenantService
            , MainDbContext mainDbContext,
            IMemoryCache memoryCache
            )
        {
            this._currentTenantService = currentTenantService;
            this._mainDbContext = mainDbContext;
            this._memoryCache = memoryCache;
        }

        public async Task<ITenantDbContext> GetTenantDbContextAsync(int tenantId)
        {
            string connectionString = await GetConnectionStringForTenantByIdAsync(tenantId);
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _currentTenantService.ConnectionString = connectionString;
            ITenantDbContext tenantDbContext = new TenantDbContext(optionsBuilder.Options, null);
            return await Task.FromResult(tenantDbContext);
        }

        public async Task<ITenantDbContext> GetTenantDbContextAsync(string tenantName)
        {
            string connectionString = await GetConnectionStringForTenantByNameAsync(tenantName);
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _currentTenantService.ConnectionString = connectionString;
            ITenantDbContext tenantDbContext = new TenantDbContext(optionsBuilder.Options, null);
            return await Task.FromResult(tenantDbContext);
        }

        public async Task SetTenantDbContextAsync(int tenantId)
        {
            string connectionString = await _memoryCache.GetOrCreateAsync(
                $"Tenant_{tenantId}",
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);
                    return GetConnectionStringForTenantByIdAsync(tenantId);
                });

            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _currentTenantService.ConnectionString = connectionString;
            await Task.CompletedTask;
        }

        public async Task SetTenantDbContextAsync(string tenantName)
        {
            string connectionString = await _memoryCache.GetOrCreateAsync(
                $"Tenant_{tenantName}",
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);
                    return GetConnectionStringForTenantByNameAsync(tenantName);
                });
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _currentTenantService.ConnectionString = connectionString;
            await Task.CompletedTask;
        }

        public async Task<string> GetConnectionStringForTenantByIdAsync(int tenantId)
        {
            var tenants = await _mainDbContext.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenants is null
                || string.IsNullOrWhiteSpace(tenants?.DataSource)
                || string.IsNullOrWhiteSpace(tenants?.Catalog)
                || string.IsNullOrWhiteSpace(tenants?.UserId)
                || string.IsNullOrWhiteSpace(tenants?.Password))
            {
                throw new Exception("Tenant Not Found");
            }
            _currentTenantService.ConnectionString = ConnectionStringProvider.GetConnectionString(tenants.DataSource, tenants.Catalog, tenants.UserId, tenants.Password);
            _currentTenantService.TenantId = tenants.Id;
            return ConnectionStringProvider.GetConnectionString(tenants.DataSource, tenants.Catalog, tenants.UserId, tenants.Password);
        }


        public async Task<string> GetConnectionStringForTenantByNameAsync(string tenantName)
        {
            var tenants = await _mainDbContext.Tenants.FirstOrDefaultAsync(x => x.Name == tenantName);
            if (tenants is null
                || string.IsNullOrWhiteSpace(tenants?.DataSource)
                || string.IsNullOrWhiteSpace(tenants?.Catalog)
                || string.IsNullOrWhiteSpace(tenants?.UserId)
                || string.IsNullOrWhiteSpace(tenants?.Password))
            {
                throw new Exception("Tenant Not Found");
            }
            _currentTenantService.ConnectionString = ConnectionStringProvider.GetConnectionString(tenants.DataSource, tenants.Catalog, tenants.UserId, tenants.Password);
            _currentTenantService.TenantId = tenants.Id;
            return ConnectionStringProvider.GetConnectionString(tenants.DataSource, tenants.Catalog, tenants.UserId, tenants.Password);
        }


    }
}
