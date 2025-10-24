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
    public class UpdateMerchantBusinessHoursFunction
    {
        private readonly IMediator _mediator;

        public UpdateMerchantBusinessHoursFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpdateMerchantBusinessHours")]
        [OpenApiOperation(operationId: "UpdateMerchantBusinessHours", tags: new[] { "Merchant Settings" }, 
            Summary = "Update merchant business hours", 
            Description = "Updates all business hours for a merchant (bulk update)")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpdateMerchantBusinessHoursRequest), 
            Description = "Updated business hours", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Business hours updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant settings not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "merchants/{merchantId}/business-hours")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpdateMerchantBusinessHoursRequest>();
            var response = req.CreateResponse();

            if (request == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid business hours data");
                return response;
            }

            var command = new UpdateMerchantBusinessHoursCommand
            {
                MerchantId = merchantId,
                BusinessHours = request.BusinessHours.Select(bh => new BusinessHourDto
                {
                    DayOfWeek = bh.DayOfWeek,
                    OpenTime = bh.OpenTime,
                    CloseTime = bh.CloseTime,
                    IsClosed = bh.IsClosed
                }).ToList()
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}