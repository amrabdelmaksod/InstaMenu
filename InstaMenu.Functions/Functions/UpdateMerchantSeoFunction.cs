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
    public class UpdateMerchantSeoFunction
    {
        private readonly IMediator _mediator;

        public UpdateMerchantSeoFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMerchantSeo")]
        [OpenApiOperation(operationId: "UpdateMerchantSeo", tags: new[] { "Merchant Settings" }, 
            Summary = "Update merchant SEO settings", 
            Description = "Updates merchant SEO settings including title and description in both languages")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateMerchantSeoRequest), 
            Description = "Updated SEO settings", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "SEO settings updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant settings not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "merchants/{merchantId}/seo")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateMerchantSeoRequest>();
            var response = req.CreateResponse();

            if (request == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid SEO data");
                return response;
            }

            var command = new UpdateMerchantSeoCommand
            {
                MerchantId = merchantId,
                SeoTitle = request.SeoTitle,
                SeoTitleAr = request.SeoTitleAr,
                SeoDescription = request.SeoDescription,
                SeoDescriptionAr = request.SeoDescriptionAr
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}