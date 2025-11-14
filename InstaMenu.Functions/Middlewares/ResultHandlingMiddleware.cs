using InstaMenu.Application.Common.Results;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;

namespace InstaMenuFunctions.Middlewares
{
    /// <summary>
    /// Middleware to automatically handle Result pattern responses
    /// Maps business Results to appropriate HTTP responses
    /// </summary>
    public class ResultHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
 await next(context);

          var httpRequestData = await context.GetHttpRequestDataAsync();
 if (httpRequestData == null) return;

         var invocationResult = context.GetInvocationResult();
      if (invocationResult?.Value == null) return;

            // Handle both Result and Result<T>
            if (IsResultType(invocationResult.Value))
 {
                await HandleResultResponse(context, httpRequestData, invocationResult.Value);
     }
        }

        private static bool IsResultType(object value)
        {
   var type = value.GetType();
        return type == typeof(Result) || 
      (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>));
        }

        private static async Task HandleResultResponse(FunctionContext context, HttpRequestData request, object result)
        {
var response = request.CreateResponse();

        if (result is Result simpleResult)
            {
await HandleSimpleResult(response, simpleResult);
      }
 else
            {
      await HandleGenericResult(response, result);
}

            context.GetInvocationResult().Value = response;
        }

        private static async Task HandleSimpleResult(HttpResponseData response, Result result)
        {
      if (result.IsSuccess)
            {
         response.StatusCode = HttpStatusCode.OK;
    }
        else
     {
                var statusCode = DetermineHttpStatusCode(result.Error);
            response.StatusCode = statusCode;
        await response.WriteAsJsonAsync(new { error = result.Error });
            }
        }

        private static async Task HandleGenericResult(HttpResponseData response, object result)
        {
     var resultType = result.GetType();
            var isSuccessProperty = resultType.GetProperty("IsSuccess")!;
       var errorProperty = resultType.GetProperty("Error")!;
            var valueProperty = resultType.GetProperty("Value")!;

   var isSuccess = (bool)isSuccessProperty.GetValue(result)!;
var error = (string)errorProperty.GetValue(result)!;

    if (isSuccess)
         {
         var value = valueProperty.GetValue(result);
      response.StatusCode = HttpStatusCode.OK;
        
    if (value != null)
  {
         await response.WriteAsJsonAsync(value);
    }
            }
            else
     {
      var statusCode = DetermineHttpStatusCode(error);
           response.StatusCode = statusCode;
   await response.WriteAsJsonAsync(new { error });
}
  }

        private static HttpStatusCode DetermineHttpStatusCode(string error)
        {
// Map common error patterns to HTTP status codes
    var lowerError = error.ToLowerInvariant();

            if (lowerError.Contains("not found"))
     return HttpStatusCode.NotFound;

   if (lowerError.Contains("already exists") || lowerError.Contains("conflict"))
        return HttpStatusCode.Conflict;

            if (lowerError.Contains("validation") || lowerError.Contains("invalid") || lowerError.Contains("required"))
       return HttpStatusCode.BadRequest;

        if (lowerError.Contains("unauthorized") || lowerError.Contains("forbidden"))
    return HttpStatusCode.Unauthorized;

   if (lowerError.Contains("database") || lowerError.Contains("unexpected"))
    return HttpStatusCode.InternalServerError;

   // Default to BadRequest for business logic errors
    return HttpStatusCode.BadRequest;
   }
    }
}