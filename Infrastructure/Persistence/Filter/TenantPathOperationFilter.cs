using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Persistence.Filter
{
    public class TenantPathOperationFilter : IOperationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantPathOperationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer" // or API key, depending on the authentication used
                        },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>() // List of required scopes, leave empty if none
                }
            }
        };
        }
    }
}
