﻿using Application.Interfaces.DbContext;
using Application.Interfaces.Provider;
using Carter;
using Domain.Settings;
using Infrastructure.Persistence.Provider;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServiceCollector.Abstractions;
using System.Text;

namespace Infrastructure;

public static class ServiceRegistration
{

    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter(configurator: c =>
        {
            c.WithValidatorLifetime(ServiceLifetime.Scoped);
        });
        services.AddHttpContextAccessor();  // Needed to access HttpContext

        services.ConfigureDbContext(configuration);
        services.ConfigureIdentity(configuration);

        services.Configure<JwtTokenSetting>(configuration.GetSection("JwtSettings"));

        // Add Tenant Migration Service
        services.AddScoped<TenantMigrationService>();
        //services.ConfigureServices();
        RepositoryServiceRegistration.ConfigureServices(services);
        // Current tenant service with scoped lifetime (created per each request)

        services.AddServices()
            .AddRepositories();
    }

    internal static  IServiceCollection AddServices(this IServiceCollection services)
    {
        // Current tenant service with scoped lifetime (created per each request)
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<ITenantUserManagementService, TenantUserManagementService>();
        services.AddScoped<ITenancyManagerService, TenancyManagerService>();
        services.AddTransient<ITokenService, TokenService>();
        return services;
    }

    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<ITenantsRepository, TenantsRepository>();
    }


    internal static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddIdentityCore<ApplicationUser>()
             .AddRoles<ApplicationRole>()
             .AddSignInManager<SignInManager<ApplicationUser>>()
             .AddEntityFrameworkStores<MainDbContext>()
             .AddDefaultTokenProviders();

        services.AddIdentityCore<TenantUser>()
                .AddSignInManager<SignInManager<TenantUser>>()
                .AddRoles<TenantRole>()
                .AddEntityFrameworkStores<TenantDbContext>()
                .AddDefaultTokenProviders();

        services.AddAuthorization();
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie(IdentityConstants.ApplicationScheme, o =>
        {
            o.LoginPath = new PathString("/Account/Login");
            o.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            };
        })
        .AddCookie(IdentityConstants.ExternalScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.ExternalScheme;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        })
        .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
            o.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
            };
        })
        .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
            o.Events = new CookieAuthenticationEvents
            {
                OnRedirectToReturnUrl = _ => Task.CompletedTask
            };
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        return services;
    }

    internal static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantContextProvider, TenantContextProvider>();
        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName));
        });
        services.AddDbContextFactory<TenantDbContext>();
        services.AddScoped<ITenantDbContext>(provider => provider.GetRequiredService<TenantDbContext>());
        return services;
    }
}


