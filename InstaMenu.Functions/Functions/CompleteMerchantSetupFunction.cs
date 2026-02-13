using InstaMenu.Application.Merchants.Commands;
using InstaMenuFunctions.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class CompleteMerchantSetupFunction
    {
        private readonly IMediator _mediator;

        public CompleteMerchantSetupFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("CompleteMerchantSetup")]
        [OpenApiOperation(operationId: "CompleteMerchantSetup", tags: new[] { "Merchant Settings" },
            Summary = "Complete merchant setup",
            Description = "Completes merchant setup by configuring slug, WhatsApp number, and currency after registration")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true,
            Type = typeof(Guid), Description = "The merchant ID to setup")]
        [OpenApiRequestBody("application/json", typeof(CompleteMerchantSetupRequest),
            Description = "Merchant setup information", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Merchant setup completed successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain",
            bodyType: typeof(string), Description = "Invalid request data or validation error")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "text/plain",
            bodyType: typeof(string), Description = "Slug already exists")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain",
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "merchants/{merchantId}/complete-setup")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<CompleteMerchantSetupRequest>();
            var response = req.CreateResponse();

            if (request == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid request data");
                return response;
            }

            var command = new CompleteMerchantSetupCommand
            {
                MerchantId = merchantId,
                Slug = request.Slug,
                WhatsAppNumber = request.WhatsAppNumber,
                Currency = request.Currency ?? "EGP"
            };

            // Validate the command using FluentValidation
            var validator = new CompleteMerchantSetupCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            
            if (!validationResult.IsValid)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                await response.WriteStringAsync(errors);
                return response;
            }

            try
            {
                var success = await _mediator.Send(command);

                response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                
                if (!success)
                {
                    await response.WriteStringAsync("Merchant not found");
                }

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = ex.Message.Contains("Slug") ? HttpStatusCode.Conflict : HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}

