using InstaMenu.Application.Orders.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class SendOrderToWhatsAppFunction
    {
        private readonly IMediator _mediator;

        public SendOrderToWhatsAppFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("SendOrderToWhatsApp")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "orders/{id:guid}/send-to-whatsapp")] HttpRequestData req,
            Guid id,
            FunctionContext context)
        {
            try
            {
                await _mediator.Send(new SendOrderToWhatsAppCommand { OrderId = id });
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync("Order sent to WhatsApp successfully");
                return response;
            }
            catch (Exception ex)
            {
                var error = req.CreateResponse(HttpStatusCode.BadRequest);
                await error.WriteStringAsync(ex.Message);
                return error;
            }
        }
    }
}