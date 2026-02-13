using InstaMenu.Application.Merchants.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class GetMerchantQRCodeFunction
    {
        private readonly IMediator _mediator;

        public GetMerchantQRCodeFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetMerchantQRCode")]
        [OpenApiOperation(operationId: "GetMerchantQRCode", tags: new[] { "Merchant Settings" },
            Summary = "Get or generate merchant QR code",
            Description = "Retrieves the merchant's QR code. If it doesn't exist, generates a new one and stores it in the database.")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true,
            Type = typeof(Guid), Description = "The merchant ID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(MerchantQRCodeDto), Description = "QR code data including menu URL and QR code image")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain",
            bodyType: typeof(string), Description = "Merchant slug not set")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain",
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "merchants/{merchantId}/qrcode")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();

            try
            {
                var query = new GetMerchantQRCodeQuery { MerchantId = merchantId };
                var result = await _mediator.Send(query);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteAsJsonAsync(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    await response.WriteStringAsync(ex.Message);
                }
                else if (ex.Message.Contains("slug"))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    await response.WriteStringAsync(ex.Message);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    await response.WriteStringAsync($"An error occurred: {ex.Message}");
                }
            }

            return response;
        }
    }
}
