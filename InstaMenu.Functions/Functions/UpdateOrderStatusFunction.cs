using InstaMenu.Application.Orders.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class UpdateOrderStatusFunction
    {
        private readonly IMediator _mediator;

        public UpdateOrderStatusFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateOrderStatus")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "orders/{orderId}/status")] HttpRequestData req,
            Guid orderId,
            FunctionContext executionContext)
        {
            var command = await req.ReadFromJsonAsync<UpdateOrderStatusCommand>();
            var response = req.CreateResponse();

            if (command == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid request body");
                return response;
            }

            command.OrderId = orderId;

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}