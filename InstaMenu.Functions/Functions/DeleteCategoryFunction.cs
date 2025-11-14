using InstaMenu.Application.Categories.Commands;
using InstaMenu.Application.Common.Results;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class DeleteCategoryFunction
    {
        private readonly IMediator _mediator;

        public DeleteCategoryFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("DeleteCategory")]
        [OpenApiOperation(operationId: "DeleteCategory", tags: new[] { "Categories" }, Summary = "Delete a category", Description = "Deletes a menu category and all its menu items")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The category ID to delete")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Category deleted successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object), Description = "Category not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Cannot delete category with menu items")]
        public async Task<Result> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "categories/{id}")] HttpRequestData req,
            Guid id,
            FunctionContext executionContext)
        {
            var command = new DeleteCategoryCommand { CategoryId = id };
            return await _mediator.Send(command);
        }
    }
}