namespace Infrastructure.Persistence.Repositories
{
    public class TenantsRepository : Repository<Tenants>, ITenantsRepository
    {
        public TenantsRepository(MainDbContext dbContext) : base(dbContext)
        {
        }
    }
}
