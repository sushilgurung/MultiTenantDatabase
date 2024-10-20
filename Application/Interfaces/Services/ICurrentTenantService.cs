

namespace Application.Interfaces.Services
{
    public interface ICurrentTenantService
    {
        string ConnectionString { get; set; }
        int TenantId { get; set; }
    }
}
