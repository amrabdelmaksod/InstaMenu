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
    public class UpsertMerchantSocialLinkFunction
    {
        private readonly IMediator _mediator;

        public UpsertMerchantSocialLinkFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpsertMerchantSocialLink")]
        [OpenApiOperation(operationId: "UpsertMerchantSocialLink", tags: new[] { "Merchant Settings" }, 
            Summary = "Add or update a single social media link", 
            Description = "Creates a new social media link or updates an existing one for the specified platform")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpsertMerchantSocialLinkRequest), 
            Description = "Social media link details", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Social link created/updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "merchants/{merchantId}/social-links")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpsertMerchantSocialLinkRequest>();
            var response = req.CreateResponse();

            if (request == null || string.IsNullOrWhiteSpace(request.Platform) || string.IsNullOrWhiteSpace(request.Url))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid social link data - Platform and Url are required");
                return response;
            }

            var command = new UpsertMerchantSocialLinkCommand
            {
                MerchantId = merchantId,
                Platform = request.Platform,
                Url = request.Url,
                DisplayOrder = request.DisplayOrder
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}