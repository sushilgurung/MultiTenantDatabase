using Domain.Configurations.MainConfigurations;

namespace Domain.Configurations.TenantConfigurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Products>, IMainDbContextConfig
    {
        public void Configure(EntityTypeBuilder<Products> builder)
        {
         builder.HasOne<Categories>(x=>x.Categories).WithMany(x=>x.Products).HasForeignKey(x=>x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
