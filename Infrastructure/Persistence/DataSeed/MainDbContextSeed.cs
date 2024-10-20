using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.DataSeed.FakeDataGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.DataSeed
{
    public static class MainDbContextSeed
    {
        public static async Task InitializeDatabaseAsync(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                using (var _context = scope.ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    try
                    {
                        if (_context.Database.IsSqlServer())
                        {
                            await _context.Database.MigrateAsync();
                        }
                     //   await AddTenants(_context);
                    }
                    catch (Exception)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            }
        }


        public static async Task AddTenants(MainDbContext _context)
        {
            var tenantsGenerator = DataGenerator.GetTenantsGenerator();
            var generatedTenants = tenantsGenerator.Generate(1);
            List<Tenants> tenants = new List<Tenants>();
            tenants.AddRange(generatedTenants);

            foreach (var item in tenants)
            {
                var details = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (details is not null)
                {
                    //details.Name = item.Name;
                    //details.BusinessName = item.BusinessName;
                    //details.Logo = item.Logo;
                    details.DataSource = item.DataSource;
                    //  details.Catalog = item.Catalog;
                    details.UserId = item.UserId;
                    details.Password = item.Password;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.Database.OpenConnectionAsync();
                    await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [dbo].[{nameof(Tenants)}] ON;");
                    await _context.Tenants.AddAsync(item);
                    await _context.SaveChangesAsync();
                    await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [dbo].[{nameof(Tenants)}] OFF;");
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }
    }
}
