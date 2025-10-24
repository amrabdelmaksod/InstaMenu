using InstaMenu.Application.Merchants.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class DeleteMerchantSocialLinkFunction
    {
        private readonly IMediator _mediator;

        public DeleteMerchantSocialLinkFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("DeleteMerchantSocialLink")]
        [OpenApiOperation(operationId: "DeleteMerchantSocialLink", tags: new[] { "Merchant Settings" }, 
            Summary = "Delete a social media link", 
            Description = "Soft deletes a social media link for the specified platform")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID")]
        [OpenApiParameter(name: "platform", In = ParameterLocation.Path, Required = true, 
            Type = typeof(string), Description = "The social media platform to delete")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Social link deleted successfully")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Social link not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "merchants/{merchantId}/social-links/{platform}")] HttpRequestData req,
            Guid merchantId,
            string platform,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();

            if (string.IsNullOrWhiteSpace(platform))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Platform parameter is required");
                return response;
            }

            var command = new DeleteMerchantSocialLinkCommand
            {
                MerchantId = merchantId,
                Platform = platform
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
            return response;
        }
    }
}