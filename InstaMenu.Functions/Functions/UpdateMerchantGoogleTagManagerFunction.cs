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
    public class UpdateMerchantGoogleTagManagerFunction
    {
        private readonly IMediator _mediator;

        public UpdateMerchantGoogleTagManagerFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMerchantGoogleTagManager")]
        [OpenApiOperation(operationId: "UpdateMerchantGoogleTagManager", tags: new[] { "Merchant Settings" }, 
            Summary = "Update Google Tag Manager settings", 
            Description = "Updates merchant Google Tag Manager configuration")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateMerchantGoogleTagManagerRequest), 
            Description = "Updated Google Tag Manager settings", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Google Tag Manager settings updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant settings not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "merchants/{merchantId}/google-tag-manager")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateMerchantGoogleTagManagerRequest>();
            var response = req.CreateResponse();

            if (request == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid Google Tag Manager data");
                return response;
            }

            var command = new UpdateMerchantGoogleTagManagerCommand
            {
                MerchantId = merchantId,
                GoogleTagManagerId = request.GoogleTagManagerId,
                IsGoogleTagManagerEnabled = request.IsGoogleTagManagerEnabled
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}