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
    public class UpdateMerchantSocialLinksFunction
    {
        private readonly IMediator _mediator;

        public UpdateMerchantSocialLinksFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMerchantSocialLinks")]
        [OpenApiOperation(operationId: "UpdateMerchantSocialLinks", tags: new[] { "Merchant Settings" }, 
            Summary = "Update merchant social media links", 
            Description = "Updates all social media links for a merchant (bulk update)")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateMerchantSocialLinksRequest), 
            Description = "Updated social media links", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Social links updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "merchants/{merchantId}/social-links")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateMerchantSocialLinksRequest>();
            var response = req.CreateResponse();

            if (request == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid social links data");
                return response;
            }

            var command = new UpdateMerchantSocialLinksCommand
            {
                MerchantId = merchantId,
                SocialLinks = request.SocialLinks.Select(sl => new SocialLinkDto
                {
                    Platform = sl.Platform,
                    Url = sl.Url,
                    DisplayOrder = sl.DisplayOrder
                }).ToList()
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}