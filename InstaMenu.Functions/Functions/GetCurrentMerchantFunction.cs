using InstaMenu.Application.Auth.Queries;
using InstaMenuFunctions.extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace InstaMenuFunctions.Functions
{
    public class GetCurrentMerchantFunction
    {
        private readonly IMediator _mediator;

        public GetCurrentMerchantFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetCurrentMerchant")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "auth/me")] HttpRequestData req,
            FunctionContext ctx)
        {
            if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                return new EmptyResult();
            }

            var user = ctx.GetUser();
            if (user == null || user.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return new UnauthorizedResult();
            }

            var merchantId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var merchantDto = await _mediator.Send(new GetCurrentMerchantQuery { MerchantId = merchantId });

                return new OkObjectResult(merchantDto);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }
    }
}
