
namespace Infrastructure.Persistence.Repositories
{
    public class CategoriesRepository : Repository<Categories>, ICategoriesRepository
    {
        public CategoriesRepository(TenantDbContext dbContext) : base(dbContext)
        {
        }
    }
}
