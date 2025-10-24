using InstaMenu.Application.Orders.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

public class GetOrderByIdFunction
{
    private readonly IMediator _mediator;

    public GetOrderByIdFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("GetOrderById")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{id:guid}")] HttpRequestData req,
        Guid id,
        FunctionContext executionContext)
    {
        var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        var response = req.CreateResponse();

        if (result == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            await response.WriteStringAsync("Order not found");
            return response;
        }

        await response.WriteAsJsonAsync(result);
        return response;
    }
}
