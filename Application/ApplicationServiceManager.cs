using Application.Common.Behaviours;
using Application.Exceptions;
using Application.Features.Tenant.Command.TenantAdd;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Gurung.ServiceRegister;
namespace Application
{
    public class ApplicationServiceManager : IServicesRegistration
    {
        public void AddServices(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()));
            services.AddSingleton<ApiExceptionFilter>();
        }
    }
}
