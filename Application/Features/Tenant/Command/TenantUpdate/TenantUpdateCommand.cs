using Application.Common.Extension;
using Application.Features.Tenant.Command.TenantAdd;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Bogus.DataSets;
using Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Command.TenantUpdate
{
    public record TenantUpdateCommand(
    [property: JsonIgnore] int TenantId,
    string Name,
    string BusinessName,
    string Logo,
    string DataSource,
    string UserId,
    string Password
    ) : IRequest<IResult>;

    public class TenantUpdateCommandHandler(
                    ITenantsRepository _tenantsRepository,
                    ITenancyManagerService _tenancyManagerService,
                    ILogger<TenantAddCommandHandler> _logger,
                    IMemoryCache _memoryCache
                    ) : IRequestHandler<TenantUpdateCommand, IResult>
    {
        public async Task<IResult> Handle(TenantUpdateCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for {RequestData}", nameof(TenantUpdateCommandHandler), JsonConvert.SerializeObject(new
            {
                Name = request.Name,
                BusinessName = request.BusinessName,
                Logo = request.Logo,
                DataSource = request.DataSource,
                UserId = request.UserId,
            }));

            try
            {
                var tenants = await _tenantsRepository.GetByIdAsync(request.TenantId);
                if (tenants is null)
                {
                    return Results.NotFound("Tenant Not Found");
                }

                _memoryCache.Remove($"Tenant_{tenants.Id}");
                _memoryCache.Remove($"Tenant_{tenants.Name}");

                bool renameResult = await _tenancyManagerService.RenameDatabaseAsync(
                    request.DataSource,
                    request.UserId,
                    request.Password,
                    tenants.Catalog,
                    request.Name.SanitizeTenant()
                    );
                _logger.LogInformation("{FunctionName} trigger function {Function} returned a response {Response}",
                nameof(TenantUpdateCommandHandler), nameof(_tenancyManagerService.RenameDatabaseAsync), renameResult);

                if (!renameResult)
                {
                    return Results.BadRequest(new
                    {
                        Message = "Failed to rename Tenant Database. Please check the credentials."
                    });
                }

                tenants.Name = request.Name;
                tenants.BusinessName = request.BusinessName;
                tenants.Logo = request.Logo;
                tenants.DataSource = request.DataSource;
                tenants.Catalog = request.Name.SanitizeTenant();
                tenants.UserId = request.UserId;
                tenants.Password = request.Password;
                await _tenantsRepository.UpdateAsync(tenants);
                int result = await _tenantsRepository.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("{FunctionName} trigger {Tenant} has inserted {RequestData}", nameof(TenantUpdateCommand), nameof(tenants), new
                    {
                        Name = request.Name,
                        BusinessName = request.BusinessName,
                        Logo = request.Logo,
                        DataSource = request.DataSource,
                    });
                    return Results.Ok(new
                    {
                        Message = "Tenant has been updated successfully."
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
                        Message = "Failed to updated Tenant. Please try again."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {FunctionName}. Request data: {@RequestData}", nameof(TenantUpdateCommand), new
                {
                    Name = request.Name,
                    BusinessName = request.BusinessName,
                    Logo = request.Logo,
                    DataSource = request.DataSource,
                    UserId = request.UserId,
                });
                return Results.BadRequest(new
                {
                    Message = "Failed to updated Tenant. Please try again."
                });
            }
        }
    }

    public class TenantUpdateCommandValidator : AbstractValidator<TenantUpdateCommand>
    {
        private readonly ITenantsRepository _tenantsRepository;
        public TenantUpdateCommandValidator(ITenantsRepository tenantsRepository)
        {
            this._tenantsRepository = tenantsRepository;
            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(async (model, name, cancellationToken) =>
                {
                    return await HasNameExistAsync(name, model.TenantId, cancellationToken);
                })
                .WithMessage("Tenant Name Already Exist");

            RuleFor(x => x.BusinessName).NotEmpty();
            RuleFor(x => x.Logo).NotEmpty();
            RuleFor(x => x.DataSource).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }

        private async Task<bool> HasNameExistAsync(string name, int tenantId, CancellationToken cancellationToken)
        {
            return !await _tenantsRepository.AnyAsync(x => x.Name.ToUpper() == name.Trim().ToUpper() && x.Id != tenantId);
        }
    }
}
