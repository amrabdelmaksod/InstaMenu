using System.Net;
using InstaMenu.Application.Index.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace InstaMenuFunctions.Functions
{
    public class GetIndexFunction
    {
        private readonly IMediator _mediator;

        public GetIndexFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetIndex")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/index")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var result = await _mediator.Send(new GetIndexQuery());

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }
}
