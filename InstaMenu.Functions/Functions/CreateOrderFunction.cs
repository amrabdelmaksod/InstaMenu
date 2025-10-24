using MediatR;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using InstaMenu.Application.Orders.Commands;

public class CreateOrderFunction
{
    private readonly IMediator _mediator;

    public CreateOrderFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("CreateOrder")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var command = await req.ReadFromJsonAsync<CreateOrderCommand>();
        if (command == null)
        {
            var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
            await badReq.WriteStringAsync("Invalid body");
            return badReq;
        }

        try
        {
            var result = await _mediator.Send(command);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            var errorRes = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorRes.WriteStringAsync(ex.Message);
            return errorRes;
        }
    }
}
