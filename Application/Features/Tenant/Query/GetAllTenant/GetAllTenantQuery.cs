using Application.Dto;
using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Query.GetAllTenant
{
    public record GetAllTenantQuery : IRequest<IResult>;


    public class GetAllTenantQueryHandler(ITenantsRepository tenantsRepository) : IRequestHandler<GetAllTenantQuery, IResult>
    {
        public async Task<IResult> Handle(GetAllTenantQuery request, CancellationToken cancellationToken)
        {
            var tenants = await (tenantsRepository
                .Queryable
                .Select(x => new TenantsGetAllDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    BusinessName = x.BusinessName
                }).ToListAsync(cancellationToken));
            return Results.Ok(new
            {
                Message = "Get All Tenant Success",
                data = tenants
            });
        }
    }


}
