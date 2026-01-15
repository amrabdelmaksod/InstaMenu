using InstaMenu.Application.Categories.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace InstaMenuFunctions.Functions
{
    public class DeleteCategoryFunction
    {
        private readonly IMediator _mediator;

        public DeleteCategoryFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("DeleteCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "categories/{id}")] HttpRequestData req,
            Guid id,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();

            try
            {
                var command = new DeleteCategoryCommand { CategoryId = id };
                var success = await _mediator.Send(command);

                if (!success)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    await response.WriteStringAsync("Category not found");
                    return response;
                }

                response.StatusCode = HttpStatusCode.NoContent;
                return response;
            }
            catch (InvalidOperationException ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}