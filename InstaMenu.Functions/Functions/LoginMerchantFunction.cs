using InstaMenu.Application.Auth.Commands;
using InstaMenuFunctions.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

public class LoginMerchantFunction
{
    private readonly IMediator _mediator;

    public LoginMerchantFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("LoginMerchant")]
    [OpenApiOperation(operationId: "LoginMerchant", tags: new[] { "Authentication" }, Summary = "Login merchant", Description = "Authenticates a merchant and returns an authentication token")]
    [OpenApiRequestBody("application/json", typeof(LoginMerchantRequest), Description = "Merchant login credentials", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginMerchantResponse), Description = "Login successful")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid credentials")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "auth/login")] HttpRequestData req,
        FunctionContext ctx)
    {
        var command = await req.ReadFromJsonAsync<LoginMerchantCommand>();
        var response = req.CreateResponse();

        if (command == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            await response.WriteStringAsync("Invalid request");
            return response;
        }

        var token = await _mediator.Send(command);

        if (token == null)
        {
            response.StatusCode = HttpStatusCode.Unauthorized;
            await response.WriteStringAsync("Invalid credentials");
            return response;
        }

        await response.WriteAsJsonAsync(new LoginMerchantResponse { Token = token });
        return response;
    }
}