using InstaMenu.Application.Auth.Commands;
using InstaMenu.Application.Common.Results;
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
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(object), Description = "Invalid credentials")]
    public async Task<Result<LoginMerchantResponse>> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "auth/login")] HttpRequestData req,
        FunctionContext ctx)
    {
        var command = await req.ReadFromJsonAsync<LoginMerchantCommand>();

        if (command == null)
        {
            return Result<LoginMerchantResponse>.Failure(ResultErrors.BadRequest.InvalidData());
        }

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return Result<LoginMerchantResponse>.Failure(result.Error);
        }

        var response = new LoginMerchantResponse
        {
            Token = result.Value!.Token,
            MerchantId = result.Value.MerchantId,
            Name = result.Value.Name
        };

        return Result.Success(response);
    }
}