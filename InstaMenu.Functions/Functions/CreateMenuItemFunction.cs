using InstaMenu.Application.MenuItems.Commands;
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
    public class CreateMenuItemFunction
    {
        private readonly IMediator _mediator;

        public CreateMenuItemFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("CreateMenuItem")]
        [OpenApiOperation(operationId: "CreateMenuItem", tags: new[] { "Menu Items" }, Summary = "Create a new menu item", Description = "Creates a new menu item for a specific category")]
        [OpenApiRequestBody("application/json", typeof(CreateMenuItemRequest), Description = "Menu item details", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CreateMenuItemResponse), Description = "Menu item created successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Invalid item data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(object), Description = "Authentication required")]
        public async Task<Result<CreateMenuItemResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu-items")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<CreateMenuItemCommand>();

            if (command == null)
            {
                return Result<CreateMenuItemResponse>.Failure(ResultErrors.BadRequest.InvalidData());
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return Result<CreateMenuItemResponse>.Failure(ResultErrors.BadRequest.MissingRequiredFields("Name"));
            }

            if (command.Price <= 0)
            {
                return Result<CreateMenuItemResponse>.Failure(ResultErrors.Validation.InvalidPrice(command.Price));
            }

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return Result<CreateMenuItemResponse>.Failure(result.Error);
            }

            var response = new CreateMenuItemResponse { Id = result.Value };
            return Result.Success(response);
        }
    }
}