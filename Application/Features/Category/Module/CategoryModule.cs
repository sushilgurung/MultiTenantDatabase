using Application.Features.Category.Command.CategoryAdd;
using Application.Features.Tenant.Command.TenantAdd;
using Application.Features.Tenant.Command.TenantUpdate;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Category.Module
{
    public class CategoryModule : CarterModule
    {
        public CategoryModule() : base("/api")
        {
            WithTags("Cateogry");
            IncludeInOpenApi();
            RequireAuthorization();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app
               .MapGroup("Cateogry")
               .MapPost("/Add", (IMediator mediator, CategoryAddCommand command) =>
               {
                   return mediator.Send(command);
               });
        }
    }
}
