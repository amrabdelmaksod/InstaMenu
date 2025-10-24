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
    public class CreateCategoryFunction
    {
        private readonly IMediator _mediator;

        public CreateCategoryFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("CreateCategory")]
        [OpenApiOperation(operationId: "CreateCategory", tags: new[] { "Categories" }, Summary = "Create a new category", Description = "Creates a new menu category for a merchant")]
        [OpenApiRequestBody("application/json", typeof(CreateCategoryRequest), Description = "Category details", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CreateCategoryResponse), Description = "Category created successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid category data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "categories")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<CreateCategoryCommand>();
            var response = req.CreateResponse();

            if (command == null || string.IsNullOrWhiteSpace(command.Name))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid category data");
                return response;
            }

            var newCategoryId = await _mediator.Send(command);

            response.StatusCode = HttpStatusCode.Created;
            await response.WriteAsJsonAsync(new CreateCategoryResponse { Id = newCategoryId });

            return response;
        }
    }
}