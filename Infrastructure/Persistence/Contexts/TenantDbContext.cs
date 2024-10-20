using Application.Interfaces.DbContext;
using Domain.Configurations;
using Domain.Configurations.MainConfigurations;

namespace Infrastructure.Persistence.Contexts;

public class TenantDbContext : IdentityDbContext<TenantUser, TenantRole, string>, ITenantDbContext
{
    private readonly ICurrentTenantService _tenantService;
    public string ConnectionString { get; set; }
    //public TenantDbContext(DbContextOptions<TenantDbContext> options)
    // : base(options)
    //{
    //}
    public TenantDbContext(DbContextOptions<TenantDbContext> options,
        ICurrentTenantService tenantService)
   : base(options)
    {
        this._tenantService = tenantService;
        ConnectionString = this._tenantService?.ConnectionString;
    }
    public DbSet<Balances> Balances { get; set; }

    public DbSet<Products> Products { get; set; }
    public DbSet<Categories> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(ConnectionString))
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    public async Task<bool> IsConnectionStringValidAsync()
    {
        try
        {
            await Database.OpenConnectionAsync();
            await Database.CloseConnectionAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Handle the exception and return false if the connection fails.
            Console.WriteLine($"Connection failed: {ex.Message}");
            return false;
        }
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var configType = typeof(ITenantDbContextConfig);
        var assembly = configType.Assembly;
        string namespaceName = configType.Namespace;

        var configurations = assembly.GetTypes()
            .Where(t => t.Namespace == namespaceName
            && typeof(IEntityTypeConfiguration<>).IsAssignableFrom(t)
            && !t.IsInterface
            && !t.IsAbstract);

        foreach (var config in configurations)
        {
            var configurationInstance = Activator.CreateInstance(config);
            builder.ApplyConfiguration((dynamic)configurationInstance);
        }
        base.OnModelCreating(builder);
    }
}

