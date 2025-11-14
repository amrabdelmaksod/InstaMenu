using InstaMenu.Application.Categories.Commands;
using InstaMenu.Application.Common.Results;
using InstaMenuFunctions.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class UpdateCategoryFunction
    {
        private readonly IMediator _mediator;

        public UpdateCategoryFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateCategory")]
        [OpenApiOperation(operationId: "UpdateCategory", tags: new[] { "Categories" }, Summary = "Update a category", Description = "Updates an existing menu category")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The category ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateCategoryRequest), Description = "Updated category details", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Category updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Invalid category data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Category not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(object), Description = "Authentication required")]
        public async Task<Result> Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "categories/{id}")] HttpRequestData req,
     Guid id,
 FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateCategoryRequest>();

            if (request == null)
            {
                return Result.Failure(ResultErrors.BadRequest.InvalidData());
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure(ResultErrors.BadRequest.MissingRequiredFields("Name"));
            }

            var command = new UpdateCategoryCommand
            {
                CategoryId = id,
                Name = request.Name,
                SortOrder = request.SortOrder
            };

            return await _mediator.Send(command);
        }
    }
}