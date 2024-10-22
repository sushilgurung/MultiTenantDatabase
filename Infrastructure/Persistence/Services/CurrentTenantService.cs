

namespace Infrastructure.Persistence.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public string ConnectionString { get; set; }
        public int TenantId { get; set; }

    }
}
