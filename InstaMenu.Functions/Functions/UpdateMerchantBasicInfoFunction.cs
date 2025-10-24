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
    public class UpdateMerchantBasicInfoFunction
    {
        private readonly IMediator _mediator;

        public UpdateMerchantBasicInfoFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMerchantBasicInfo")]
        [OpenApiOperation(operationId: "UpdateMerchantBasicInfo", tags: new[] { "Merchant Settings" }, 
            Summary = "Update merchant basic information", 
            Description = "Updates basic merchant information including name, slug, and status")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateMerchantBasicInfoRequest), 
            Description = "Updated merchant basic information", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Merchant basic info updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "merchants/{merchantId}/basic-info")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateMerchantBasicInfoRequest>();
            var response = req.CreateResponse();

            if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid merchant data - Name and Slug are required");
                return response;
            }

            var command = new UpdateMerchantCommand
            {
                MerchantId = merchantId,
                Name = request.Name,
                NameAr = request.NameAr,
                Slug = request.Slug,
                Status = request.Status
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}