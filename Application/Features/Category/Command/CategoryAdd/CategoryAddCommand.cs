using Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Category.Command.CategoryAdd
{
    public record CategoryAddCommand(string Name, int DisplayOrder) : IRequest<IResult>;

    public class CategoryAddCommandHandler(ICategoriesRepository _categoriesRepository) : IRequestHandler<CategoryAddCommand, IResult>
    {
        public async Task<IResult> Handle(CategoryAddCommand request, CancellationToken cancellationToken)
        {
            Categories categories = new Categories()
            {
                Name = request.Name,
                DisplayOrder = request.DisplayOrder,    
            };
            await _categoriesRepository.AddAsync(categories);
            var result = await _categoriesRepository.SaveChangesAsync();
            if (categories.Id > 0)
            {
                return Results.Ok(new
                {
                    Message = "Category has been Added"
                });
            }
            else
            {
                return Results.BadRequest(new
                {
                    Message = "Failed to add category. Please try again."
                });
            }
        }
    }
}
