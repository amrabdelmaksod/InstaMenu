using InstaMenu.Application.Auth.Commands;
using InstaMenu.Application.Common.Results;
using InstaMenuFunctions.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class RegisterMerchantFunction
    {
        private readonly IMediator _mediator;

        public RegisterMerchantFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("RegisterMerchant")]
        [OpenApiOperation(operationId: "RegisterMerchant", tags: new[] { "Authentication" }, Summary = "Register a new merchant", Description = "Creates a new merchant account and returns an authentication token")]
        [OpenApiRequestBody("application/json", typeof(RegisterMerchantRequest), Description = "Merchant registration details", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(RegisterMerchantResponse), Description = "Merchant registered successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Invalid registration data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "application/json", bodyType: typeof(object), Description = "Phone number or slug already exists")]
        public async Task<Result<RegisterMerchantResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "auth/register")] HttpRequestData req,
      FunctionContext ctx)
        {
            var command = await req.ReadFromJsonAsync<RegisterMerchantCommand>();

      if (command == null)
            {
        return Result<RegisterMerchantResponse>.Failure(ResultErrors.BadRequest.InvalidData());
     }

          if (string.IsNullOrWhiteSpace(command.PhoneNumber))
            {
return Result<RegisterMerchantResponse>.Failure(ResultErrors.BadRequest.MissingRequiredFields("PhoneNumber"));
            }

            if (string.IsNullOrWhiteSpace(command.Password))
        {
         return Result<RegisterMerchantResponse>.Failure(ResultErrors.BadRequest.MissingRequiredFields("Password"));
          }

            if (string.IsNullOrWhiteSpace(command.Name))
      {
  return Result<RegisterMerchantResponse>.Failure(ResultErrors.BadRequest.MissingRequiredFields("Name"));
   }

    if (string.IsNullOrWhiteSpace(command.Slug))
            {
                return Result<RegisterMerchantResponse>.Failure(ResultErrors.BadRequest.MissingRequiredFields("Slug"));
         }

          var result = await _mediator.Send(command);

         if (result.IsFailure)
   {
          return Result<RegisterMerchantResponse>.Failure(result.Error);
            }

            var response = new RegisterMerchantResponse 
            { 
   MerchantId = result.Value!.MerchantId,
      Token = result.Value.Token 
            };

            return Result.Success(response);
        }
    }
}
