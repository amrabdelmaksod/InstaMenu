using InstaMenu.Application.Auth.Commands;
using InstaMenuFunctions.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System;
using System.Threading.Tasks;

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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid registration data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "text/plain", bodyType: typeof(string), Description = "Phone number or slug already exists")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "auth/register")] HttpRequestData req,
            FunctionContext ctx)
        {
            // Handle actual POST request
            var command = await req.ReadFromJsonAsync<RegisterMerchantCommand>();
            var response = req.CreateResponse();

            if (command == null || string.IsNullOrWhiteSpace(command.PhoneNumber) || string.IsNullOrWhiteSpace(command.Password))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid registration data");
                return response;
            }

            try
            {
                var result = await _mediator.Send(command);

                response.StatusCode = HttpStatusCode.Created;
                await response.WriteAsJsonAsync(new RegisterMerchantResponse { Token = result.Token });

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}
