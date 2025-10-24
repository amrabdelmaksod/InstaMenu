using InstaMenu.Application.Merchants.Queries;
using InstaMenu.Application.Merchants.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

public class GetMenuBySlugFunction
{
    private readonly IMediator _mediator;

    public GetMenuBySlugFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("GetMenuBySlug")]
    [OpenApiOperation(operationId: "GetMenuBySlug", tags: new[] { "Menu" }, Summary = "Get merchant menu by slug", Description = "Retrieves the complete menu for a merchant using their unique slug")]
    [OpenApiParameter(name: "slug", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The unique slug identifier for the merchant")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetMenuBySlugResponse), Description = "Menu retrieved successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Merchant not found")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/{slug}")] HttpRequestData req,
        string slug,
        FunctionContext executionContext)
    {
        var query = new GetMenuBySlugQuery { Slug = slug };
        var result = await _mediator.Send(query);

        var response = req.CreateResponse();

        if (result == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            await response.WriteStringAsync("Merchant not found");
            return response;
        }

        await response.WriteAsJsonAsync(result);
        return response;
    }
}
