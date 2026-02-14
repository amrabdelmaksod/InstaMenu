using System.Net;
using InstaMenu.Application.MenuItems.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace InstaMenuFunctions.Functions
{
    public class GetMenuItemsFunction
    {
        private readonly IMediator _mediator;

        public GetMenuItemsFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetMenuItems")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/{merchantId}/menu-items")] HttpRequestData req,
            Guid merchantId,
            FunctionContext executionContext)
        {
            // optional categoryId query parameter
            Guid? categoryId = null;
            var q = req.Url.Query;
            if (!string.IsNullOrEmpty(q))
            {
                foreach (var part in q.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = part.Split('=', 2);
                    if (kv.Length == 2 && string.Equals(kv[0], "categoryId", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Guid.TryParse(Uri.UnescapeDataString(kv[1]), out var cid)) categoryId = cid;
                        break;
                    }
                }
            }

            var query = new GetMenuItemsQuery { MerchantId = merchantId, CategoryId = categoryId };
            var result = await _mediator.Send(query);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }
}
