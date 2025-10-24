using InstaMenu.Application.Categories.Commands;
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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid category data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Category not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "categories/{id}")] HttpRequestData req,
            Guid id,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<UpdateCategoryCommand>();
            var response = req.CreateResponse();

            if (command == null || string.IsNullOrWhiteSpace(command.Name))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid category data");
                return response;
            }

            command.CategoryId = id;

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}