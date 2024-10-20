using Application.Features.Category.Command.CategoryAdd;
using Application.Features.Tenant.Command.TenantAdd;
using Application.Features.Tenant.Command.TenantLogin;
using Application.Features.Tenant.Command.TenantUpdate;
using Application.Features.Tenant.Command.TenantUserRegistration;
using Application.Features.Tenant.Query.GetAllTenant;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.Tenant.Endpoint
{
    //public class Endpoint : ICarterModule
    //{
    //    public void AddRoutes(IEndpointRouteBuilder app)
    //    {
    //        app
    //            .MapGroup("Currency")
    //            .WithTags("/currencies")
    //            .MapPost("/Add", (IMediator mediator, TenantAddCommand command) =>
    //        {
    //            return mediator.Send(command);
    //        });

    //    }
    //}
    public class TenantModule : CarterModule
    {
        public TenantModule()
            : base("/api")
        {
            WithTags("Tenant");
            IncludeInOpenApi();
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app = app.MapGroup("Tenant");

            app
                .MapPost("/Add", (IMediator mediator, TenantAddCommand command) =>
            {
                return mediator.Send(command);
            });

            app.MapPut("/update/{tenantId:int}", (int tenantId, IMediator mediator, TenantUpdateCommand command) =>
            {
                var updatedCommand = command with { TenantId = tenantId };
                return mediator.Send(updatedCommand);
            });

            app.MapGet("/GetAll", (IMediator mediator) =>
            {
                return mediator.Send(new GetAllTenantQuery());
            });


            app.MapPost("/Register", (IMediator mediator, TenantUserRegistrationCommand command) =>
            {
                return mediator.Send(command);
            });

            app.MapPost("/login", (IMediator mediator, TenantLoginCommand command) =>
            {
                return mediator.Send(command);
            });
        }
    }

}
