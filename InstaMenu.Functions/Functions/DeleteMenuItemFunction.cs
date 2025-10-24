using InstaMenu.Application.MenuItems.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class DeleteMenuItemFunction
    {
        private readonly IMediator _mediator;

        public DeleteMenuItemFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("DeleteMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu-items/{id}")] HttpRequestData req,
            Guid id,
            FunctionContext executionContext)
        {
            var command = new DeleteMenuItemCommand { MenuItemId = id };
            var success = await _mediator.Send(command);

            var response = req.CreateResponse();
            response.StatusCode = success ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
            return response;
        }
    }
}