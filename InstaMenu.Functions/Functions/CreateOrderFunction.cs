using MediatR;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using InstaMenu.Application.Orders.Commands;
using InstaMenu.Application.Common.Results;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

public class CreateOrderFunction
{
    private readonly IMediator _mediator;

    public CreateOrderFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("CreateOrder")]
    [OpenApiOperation(operationId: "CreateOrder", tags: new[] { "Orders" }, Summary = "Create new order", Description = "Creates a new order for a merchant")]
    [OpenApiRequestBody("application/json", typeof(CreateOrderCommand), Description = "Order details", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CreateOrderResult), Description = "Order created successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Invalid request data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object), Description = "Merchant or items not found")]
    public async Task<Result<CreateOrderResult>> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var command = await req.ReadFromJsonAsync<CreateOrderCommand>();
        
        if (command == null)
        {
            return Result<CreateOrderResult>.Failure(ResultErrors.BadRequest.InvalidData());
        }

        return await _mediator.Send(command);
    }
}
