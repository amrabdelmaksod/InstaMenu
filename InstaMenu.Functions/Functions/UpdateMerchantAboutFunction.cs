using InstaMenu.Application.Merchants.Commands;
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
    public class UpdateMerchantAboutFunction
    {
        private readonly IMediator _mediator;

        public UpdateMerchantAboutFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMerchantAbout")]
        [OpenApiOperation(operationId: "UpdateMerchantAbout", tags: new[] { "Merchant Settings" },
              Summary = "Update merchant about section",
               Description = "Updates merchant about us information in both languages")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true,
                 Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateMerchantAboutRequest),
     Description = "Updated about us information", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "About section updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
     bodyType: typeof(object), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant settings not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json",
          bodyType: typeof(object), Description = "Authentication required")]
        public async Task<Result> Run(
         [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "merchants/{merchantId}/about")] HttpRequestData req,
              Guid merchantId,
         FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateMerchantAboutRequest>();

            if (request == null)
            {
                return Result.Failure(ResultErrors.BadRequest.InvalidData("Request body is null or invalid"));
            }

            var command = new UpdateMerchantAboutCommand
            {
                MerchantId = merchantId,
                AboutUs = request.AboutUs,
                AboutUsAr = request.AboutUsAr
            };

            return await _mediator.Send(command);
        }
    }
}