
using Application.Common.Extension;
using Application.Common.Provider;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Bogus.DataSets;
using Domain.Entities;
using Domain.TenantEntities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Command.TenantAdd;

public record TenantAddCommand(
    string Name,
    string BusinessName,
    string Logo,
    string DataSource,
    string UserId,
    string Password
    ) : IRequest<IResult>;

public class TenantAddCommandHandler(
    ITenantsRepository _tenantsRepository,
    ITenancyManagerService _tenancyManagerService,
    ILogger<TenantAddCommandHandler> _logger
    ) : IRequestHandler<TenantAddCommand, IResult>
{
    public async Task<IResult> Handle(TenantAddCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{FunctionName} trigger function received a request for {@RequestData}", nameof(TenantAddCommandHandler), request);

        var addedDatabase = await _tenancyManagerService.AddTenantDatabseAsync(
        request.Name,
        request.BusinessName,
        request.Logo,
        request.DataSource,
        request.Name.SanitizeTenant(),
        request.UserId,
        request.Password);

        _logger.LogInformation("{FunctionName} trigger function {Function} returned a response {Response}",
            nameof(TenantAddCommandHandler), nameof(_tenancyManagerService.AddTenantDatabseAsync), addedDatabase);
        if (!addedDatabase)
        {
            return Results.BadRequest(new
            {
                Message = "Failed to created Tenant Database. Please check the credentials."
            });
        }

        Tenants tenants = new Tenants()
        {
            Name = request.Name,
            BusinessName = request.Name,
            Logo = request.Logo,
            DataSource = request.DataSource,
            Catalog = request.Name.SanitizeTenant(),
            UserId = request.UserId,
            Password = request.Password,
        };
        await _tenantsRepository.AddAsync(tenants);
        int result = await _tenantsRepository.SaveChangesAsync();

        if (tenants.Id > 0)
        {
            _logger.LogInformation("{FunctionName} trigger {Tenant} has inserted {RequestData}", nameof(TenantAddCommandHandler), nameof(tenants), new
            {
                Name = request.Name,
                BusinessName = request.BusinessName,
                Logo = request.Logo,
                DataSource = request.DataSource,
            });
            return Results.Ok(new
            {
                Message = "Tenant has been Added"
            });
        }
        else
        {
            _logger.LogWarning("{FunctionName} trigger {Tenant} not inserted {RequestData}", nameof(TenantAddCommandHandler), nameof(tenants), new
            {
                Name = request.Name,
                BusinessName = request.BusinessName,
                Logo = request.Logo,
                DataSource = request.DataSource,
            });
            return Results.BadRequest(new
            {
                Message = "Failed to add Tenant. Please try again."
            });
        }
    }
}

public class TenantAddCommandValidator : AbstractValidator<TenantAddCommand>
{
    private readonly ITenantsRepository _tenantsRepository;
    public TenantAddCommandValidator(ITenantsRepository tenantsRepository)
    {
        _tenantsRepository = tenantsRepository;
        RuleFor(r => r.Name)
            .NotEmpty()
            .MustAsync(HasNameExistAsync)
            .WithMessage("Tenant Name Already Exist");

        RuleFor(r => r.BusinessName).NotEmpty();
        RuleFor(r => r.DataSource).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Password).NotEmpty();
    }
    private async Task<bool> HasNameExistAsync(string name, CancellationToken cancellationToken)
    {
        return !await _tenantsRepository.AnyAsync(x => x.Name.ToUpper() == name.Trim().ToUpper());
    }

}

