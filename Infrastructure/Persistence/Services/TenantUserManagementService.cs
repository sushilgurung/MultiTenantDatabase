using Application.Interfaces.Provider;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Infrastructure.Persistence.Services
{
    public class TenantUserManagementService : ITenantUserManagementService
    {
        //private UserManager<TenantUser> _userManager;
        //private RoleManager<TenantRole> _roleManager;
        //private SignInManager<TenantUser> _signInManager;

        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantContextProvider _tenantContextProvider;
        private readonly ILogger<TenantUserManagementService> _logger;
        public TenantUserManagementService(
            //UserManager<TenantUser> userManager,
            //RoleManager<TenantRole> roleManager,
            //SignInManager<TenantUser> signInManager,
            IServiceProvider serviceProvider,
            ITenantContextProvider tenantContextProvider,
            ILogger<TenantUserManagementService> logger
            )
        {
            //this._userManager = userManager;
            //this._roleManager = roleManager;
            //this._signInManager = signInManager;
            this._serviceProvider = serviceProvider;
            this._tenantContextProvider = tenantContextProvider;
            this._logger = logger;
        }
        public async Task<bool> RegisterTenantAsync(string email, string password, int tenantId)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for {RequestData}", nameof(RegisterTenantAsync),
                new
                {
                    email = email,
                    tenantId = tenantId
                });
            try
            {
                await _tenantContextProvider.SetTenantDbContextAsync(tenantId);
                var _userManager = _serviceProvider.GetRequiredService<UserManager<TenantUser>>();
                var _roleManager = _serviceProvider.GetRequiredService<RoleManager<TenantRole>>();
                var _signInManager = _serviceProvider.GetRequiredService<SignInManager<TenantUser>>();
                var user = new TenantUser
                {
                    Email = email,
                    UserName = email
                };
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("{FunctionName} Tenant not created request for {RequestData}", nameof(RegisterTenantAsync), new
                    { email, tenantId });
                    return result.Succeeded;
                }
                // var resultRole = await _userManager.AddToRoleAsync(user, request.Role);
                var claims = new List<Claim>()
                {
                    new Claim("FirstLogin", "true", ClaimValueTypes.Boolean),
                    new Claim("Role", "Administrator"),
                    new Claim("TenantId", tenantId.ToString(), ClaimValueTypes.Integer),
                    new Claim("Permission", "CanViewDashboard")
                };
                var claimResult = await _userManager.AddClaimsAsync(user, claims);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{FunctionName} trigger function received a request for {RequestData}", nameof(RegisterTenantAsync), new
                {
                    Email = email,
                    TenantId = tenantId
                });
                return false;
            }
        }

        public async Task<(SignInResult signInResult, TenantUser user)> AuntenticationAsync(string email, string password, int tenantId)
        {
            await _tenantContextProvider.SetTenantDbContextAsync(tenantId);
            var _userManager = _serviceProvider.GetRequiredService<UserManager<TenantUser>>();
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<TenantRole>>();
            var _signInManager = _serviceProvider.GetRequiredService<SignInManager<TenantUser>>();
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(email);
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: false);
            return (result, user);
        }
    }
}
