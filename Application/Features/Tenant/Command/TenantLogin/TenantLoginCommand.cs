using Application.Interfaces.Provider;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Command.TenantLogin
{
    public record TenantLoginCommand(string userName, string password, int tenantId) : IRequest<IResult>;

    public class TenantLoginCommandHandler : IRequestHandler<TenantLoginCommand, IResult>
    {
        private readonly ITenantUserManagementService _tenantUserManagementService;
        private readonly ITenantContextProvider _tenantContextProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITokenService _tokenService;
        public TenantLoginCommandHandler(
            ITenantUserManagementService tenantUserManagementService,
            ITenantContextProvider tenantContextProvider,
            IServiceProvider serviceProvider,
            ITokenService tokenService)
        {
            this._tenantUserManagementService = tenantUserManagementService;
            this._tenantContextProvider = tenantContextProvider;
            this._serviceProvider = serviceProvider;
            this._tokenService = tokenService;
        }
        public async Task<IResult> Handle(TenantLoginCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _tenantUserManagementService.AuntenticationAsync(request.userName, request.password, request.tenantId).ConfigureAwait(false);
            if (!userResult.signInResult.Succeeded)
            {
                return Results.Ok(new
                {
                    Succeeded = false,
                    Message = userResult.ToString()
                });
            }
            await _tenantContextProvider.SetTenantDbContextAsync(request.tenantId);
            var _userManager = _serviceProvider.GetRequiredService<UserManager<TenantUser>>();
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(userResult.user).ConfigureAwait(false);
            var claims = new List<Claim> {
                            new Claim(JwtRegisteredClaimNames.UniqueName, userResult.user.UserName),
                            new Claim(JwtRegisteredClaimNames.NameId, userResult.user.UserName),
                            new Claim(JwtRegisteredClaimNames.Sub, userResult.user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim("UserName", userResult.user.UserName)
            }.Union(userClaims);
            var token = _tokenService.GenerateToken(claims.ToList());
            return Results.Ok(new
            {
                Succeeded = true,
                Token = token,
            });
        }
    }
}
