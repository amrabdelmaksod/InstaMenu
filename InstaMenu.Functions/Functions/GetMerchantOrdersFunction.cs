using InstaMenu.Application.Merchants.Queries;
using InstaMenu.Application.Orders.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class GetMerchantOrdersFunction
    {
        private readonly IMediator _mediator;

        public GetMerchantOrdersFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetMerchantOrders")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "merchant/{merchantId}/orders")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var query = new GetMerchantOrdersQuery { MerchantId = merchantId };
            var result = await _mediator.Send(query);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }
}