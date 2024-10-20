using Application.Common.Behaviours;
using Application.Exceptions;
using Application.Features.Tenant.Command.TenantAdd;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServiceCollector.Abstractions;
using System.Reflection;

namespace Application
{
    public class ApplicationServiceManager : IServiceDiscovery
    {
        public void AddServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
            serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            serviceCollection.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()));
            serviceCollection.AddSingleton<ApiExceptionFilter>();
        }
    }
}
