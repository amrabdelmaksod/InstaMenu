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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Invalid category data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(object), Description = "Authentication required")]
        public async Task<Result<CreateCategoryResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "categories")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<CreateCategoryCommand>();

            if (command == null)
            {
                return Result<CreateCategoryResponse>.Failure(ResultErrors.BadRequest.InvalidData());
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return Result<CreateCategoryResponse>.Failure(ResultErrors.BadRequest.MissingRequiredFields("Name"));
            }

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return Result<CreateCategoryResponse>.Failure(result.Error);
            }

            var response = new CreateCategoryResponse { Id = result.Value };
            return Result.Success(response);
        }
    }
}