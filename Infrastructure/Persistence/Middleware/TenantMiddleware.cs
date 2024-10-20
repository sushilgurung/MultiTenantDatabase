namespace Infrastructure.Persistence.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContextProvider tenantContextProvider, ITokenService tokenService)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                int tenantId = tokenService.GetTenantIdFromJwt(token);
                if (tenantId > 0)
                {
                    await tenantContextProvider.SetTenantDbContextAsync(tenantId);
                }
            }
        }
        await _next(context);
    }
}

