using InstaMenu.Application.MenuItems.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class UpdateMenuItemFunction
    {
        private readonly IMediator _mediator;

        public UpdateMenuItemFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "menu-items/{id}")] HttpRequestData req,
            Guid id,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<UpdateMenuItemCommand>();
            var response = req.CreateResponse();

            if (command == null || string.IsNullOrWhiteSpace(command.Name) || command.Price <= 0)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid item data");
                return response;
            }

            command.MenuItemId = id;

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}