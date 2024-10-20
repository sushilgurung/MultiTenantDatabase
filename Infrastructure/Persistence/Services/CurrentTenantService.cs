using Application.Common.Provider;
using Application.Interfaces.Provider;
using Infrastructure.Persistence.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public string ConnectionString { get; set; }
        public int TenantId { get; set; }

    }
}
