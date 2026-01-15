using InstaMenu.Application.MenuItems.Commands;
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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid item data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu-items")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<CreateMenuItemCommand>();
            var response = req.CreateResponse();

            if (command == null || string.IsNullOrWhiteSpace(command.Name) || command.Price <= 0)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid item data");
                return response;
            }

            var newItemId = await _mediator.Send(command);

            response.StatusCode = HttpStatusCode.Created;
            await response.WriteAsJsonAsync(new CreateMenuItemResponse { Id = newItemId });

            return response;
        }
    }
}