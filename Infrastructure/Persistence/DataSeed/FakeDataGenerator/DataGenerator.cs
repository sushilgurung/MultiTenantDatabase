using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.DataSeed.FakeDataGenerator
{
    public class DataGenerator
    {
        public static Faker<Tenants> GetTenantsGenerator()
        {
            return new Faker<Tenants>()
                     .RuleFor(t => t.Id, f => f.Random.Int(1, 5))  // Random integer for Id
                     .RuleFor(t => t.Name, f => Regex.Replace(f.Company.CompanyName(), @"[^a-zA-Z0-9]", "")) // Random company name for Name
                     .RuleFor(t => t.BusinessName, f => $"{f.Company.CompanyName()} {f.Company.CompanySuffix()}") // Random company suffix for BusinessName
                     .RuleFor(t => t.Logo, f => f.Internet.Avatar()) // Random image URL for Logo

                     .RuleFor(t => t.DataSource, f => "") // Random domain name for DataSource
                     .RuleFor(t => t.Catalog, f => $"Tenant_{Regex.Replace(f.Company.CompanyName(), @"[^a-zA-Z0-9]", "")}") // Random product name for Catalog
                     .RuleFor(t => t.UserId, f => "") // Random username for UserId
                     .RuleFor(t => t.Password, f => ""); // Random password for Password
        }


        public static Faker<IdentityUser> GetAdminTenants(string tenantName)
        {
            return new Faker<IdentityUser>()
                     .RuleFor(t => t.Id, f => new Guid().ToString())  // Random integer for Id
                     .RuleFor(t => t.Email, f => $"{tenantName}@gmail.com") // Random email for Email
                     .RuleFor(t => t.EmailConfirmed, f => true)
                     .RuleFor(t => t.LockoutEnabled, f => true);
        }
    }
}
