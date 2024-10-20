using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configurations.MainConfigurations
{
    public class TenantsConfiguration : IEntityTypeConfiguration<Tenants>, IMainDbContextConfig
    {
        public void Configure(EntityTypeBuilder<Tenants> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(256);
        }
    }
}
