using Application.Common.Provider;
using Application.Features.Tenant.Command.TenantAdd;
using Azure.Core;
using Bogus.DataSets;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Persistence.Services
{
    public class TenancyManagerService : ITenancyManagerService
    {
        private readonly ILogger<TenantAddCommandHandler> _logger;
        public TenancyManagerService(
            ILogger<TenantAddCommandHandler> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Add tenant database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="businessName"></param>
        /// <param name="logo"></param>
        /// <param name="dataSource"></param>
        /// <param name="catalog"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AddTenantDatabseAsync(
        string name,
        string businessName,
        string logo,
        string dataSource,
        string catalog,
        string userId,
        string password)
        {
            try
            {
                _logger.LogInformation("{FunctionName} trigger function received a request for {RequestData}", nameof(AddTenantDatabseAsync),
                new
                {
                    Name = name,
                    BusinessName = businessName,
                    Logo = logo,
                    DataSource = dataSource,
                });
                string connectionString = ConnectionStringProvider.GetConnectionString(dataSource, catalog, userId, password);
                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                bool isValid = DatabaseValidator.IsConnectionValid(dataSource, userId, password);
                if (!isValid)
                {
                    _logger.LogInformation("{FunctionName} Database connection is not valid. Request for {RequestData}", nameof(AddTenantDatabseAsync), new
                    {
                        Name = name,
                        BusinessName = businessName,
                        Logo = logo,
                        DataSource = dataSource,
                    });
                    return false;
                }
                _logger.LogInformation("{FunctionName} Database connection is valid. Request for {RequestData}", nameof(AddTenantDatabseAsync), new
                {
                    Name = name,
                    BusinessName = businessName,
                    Logo = logo,
                    DataSource = dataSource,
                });
                using (var tenantContext = new TenantDbContext(optionsBuilder.Options, null))
                {
                    await tenantContext.Database.MigrateAsync();
                    _logger.LogInformation("{FunctionName} Database Migration successfully. Request for {RequestData}", nameof(AddTenantDatabseAsync), new
                    {
                        Name = name,
                        BusinessName = businessName,
                        Logo = logo,
                        DataSource = dataSource,
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {FunctionName}. Request data: {@RequestData}", nameof(AddTenantDatabseAsync), new
                {
                    Name = name,
                    BusinessName = businessName,
                    Logo = logo,
                    DataSource = dataSource,
                });
                return false;
            }
        }



        /// <summary>
        /// Rename database
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="oldDatabaseName"></param>
        /// <param name="newDatabaseName"></param>
        /// <returns></returns>
        public async Task<bool> RenameDatabaseAsync(string dataSource, string userId, string password, string oldDatabaseName, string newDatabaseName)
        {
            try
            {
                _logger.LogInformation("{FunctionName} trigger function received a request for {RequestData}", nameof(RenameDatabaseAsync),
                new
                {
                    DataSource = dataSource,
                    UserId = userId,
                    OldDatabaseName = oldDatabaseName,
                    NewDatabaseName = newDatabaseName,
                });
                string connectionString = $"Server={dataSource};Database=master;User Id={userId};Password={password};TrustServerCertificate=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string setSingleUserCommand = $"ALTER DATABASE {oldDatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    string renameDatabaseCommand = $"ALTER DATABASE {oldDatabaseName} MODIFY NAME = {newDatabaseName};";
                    string setMultiUserCommand = $"ALTER DATABASE {newDatabaseName} SET MULTI_USER;";

                    using (SqlCommand command = new SqlCommand(renameDatabaseCommand, connection))
                    {
                        command.CommandText = setSingleUserCommand;
                        await command.ExecuteNonQueryAsync();

                        command.CommandText = renameDatabaseCommand;
                        await command.ExecuteNonQueryAsync();

                        command.CommandText = setMultiUserCommand;
                        await command.ExecuteNonQueryAsync();
                    }
                    _logger.LogInformation("{FunctionName} Database renamed successfully request for {RequestData}", nameof(RenameDatabaseAsync),
                     new
                     {
                         DataSource = dataSource,
                         UserId = userId,
                         OldDatabaseName = oldDatabaseName,
                         NewDatabaseName = newDatabaseName,
                     });
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {FunctionName}. Request data: {@RequestData}", nameof(RenameDatabaseAsync), new
                {
                    DataSource = dataSource,
                    UserId = userId,
                    OldDatabaseName = oldDatabaseName,
                    NewDatabaseName = newDatabaseName,
                });
                return false;
            }
        }

    }
}
