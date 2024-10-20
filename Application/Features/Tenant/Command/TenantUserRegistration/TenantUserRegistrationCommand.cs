using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Command.TenantUserRegistration;

public record TenantUserRegistrationCommand(
    string email,
    string password,
    int tenantId
    ) : IRequest<IResult>;

public class TenantUserRegistrationCommandHandler : IRequestHandler<TenantUserRegistrationCommand, IResult>
{
    private readonly ITenantUserManagementService  _tenantUserManagementService;
    public TenantUserRegistrationCommandHandler(ITenantUserManagementService  tenantUserManagementService)
    {
        this._tenantUserManagementService = tenantUserManagementService;
    }
    public async Task<IResult> Handle(TenantUserRegistrationCommand request, CancellationToken cancellationToken)
    {
        await _tenantUserManagementService.RegisterTenantAsync(request.email, request.password, request.tenantId);
        return Results.Ok(new
        {
            Message = "Tenant has been Added"
        });
    }
   
}



