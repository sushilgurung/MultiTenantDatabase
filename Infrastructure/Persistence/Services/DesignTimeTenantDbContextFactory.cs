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
