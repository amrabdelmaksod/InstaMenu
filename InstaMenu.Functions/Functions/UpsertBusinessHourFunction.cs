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
    public class UpsertBusinessHourFunction
    {
        private readonly IMediator _mediator;

        public UpsertBusinessHourFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("UpsertBusinessHour")]
        [OpenApiOperation(operationId: "UpsertBusinessHour", tags: new[] { "Merchant Settings" }, 
            Summary = "Add or update business hours for a specific day", 
            Description = "Creates or updates business hours for a specific day of the week")]
        [OpenApiParameter(name: "merchantId", In = ParameterLocation.Path, Required = true, 
            Type = typeof(Guid), Description = "The merchant ID to update")]
        [OpenApiRequestBody("application/json", typeof(UpsertBusinessHourRequest), 
            Description = "Business hour details for a specific day", Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Business hour created/updated successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Invalid request data")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Merchant settings not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", 
            bodyType: typeof(string), Description = "Authentication required")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "merchants/{merchantId}/business-hours")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            var request = await req.ReadFromJsonAsync<UpsertBusinessHourRequest>();
            var response = req.CreateResponse();

            if (request == null || request.DayOfWeek < 0 || request.DayOfWeek > 6)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid business hour data - DayOfWeek must be between 0-6");
                return response;
            }

            if (!request.IsClosed && request.OpenTime >= request.CloseTime)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid business hour data - OpenTime must be before CloseTime");
                return response;
            }

            var command = new UpsertBusinessHourCommand
            {
                MerchantId = merchantId,
                DayOfWeek = request.DayOfWeek,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                IsClosed = request.IsClosed
            };

            var success = await _mediator.Send(command);

            response.StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return response;
        }
    }
}