using Application.Dto;
using Application.Features.Tenant.Command.TenantAdd;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Query.GetAllTenant
{
    public record GetAllTenantQuery : IRequest<IResult>;


    public class GetAllTenantQueryHandler(ITenantsRepository tenantsRepository, ILogger<GetAllTenantQueryHandler> _logger) : IRequestHandler<GetAllTenantQuery, IResult>
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
            _logger.LogInformation("{FunctionName} trigger  returned a response {@Response}", nameof(GetAllTenantQueryHandler), tenants);
            return Results.Ok(new
            {
                Message = "Get All Tenant Success",
                data = tenants
            });
        }
    }


}
