using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InstaMenuFunctions.Middlewares
{
    public class CorsMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();

            if (httpRequestData == null)
            {
                await next(context);
                return;
            }

            // Get the request URL and method for logging
            Console.WriteLine($"[CORS] Processing request: {httpRequestData.Method} {httpRequestData.Url}");

            // Get the origin header
            string origin = null;
            if (httpRequestData.Headers.TryGetValues("Origin", out var originValues))
            {
                origin = originValues.FirstOrDefault();
                Console.WriteLine($"[CORS] Request has origin: {origin}");
            }
            else
            {
                Console.WriteLine("[CORS] Request has no Origin header");
            }

            // For OPTIONS requests (preflight), return immediately with CORS headers
            if (string.Equals(httpRequestData.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[CORS] Handling OPTIONS preflight request");

                var response = httpRequestData.CreateResponse(HttpStatusCode.NoContent);

                // Add permissive CORS headers for development
                response.Headers.Add("Access-Control-Allow-Origin", origin ?? "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS, PATCH");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept, X-Requested-With");

                if (!string.IsNullOrEmpty(origin))
                {
                    response.Headers.Add("Access-Control-Allow-Credentials", "true");
                }

                response.Headers.Add("Access-Control-Max-Age", "86400"); // 24 hours

                // Set the response and return immediately
                context.GetInvocationResult().Value = response;

                // Log headers for debugging
                foreach (var header in response.Headers)
                {
                    Console.WriteLine($"[CORS] Response header: {header.Key}={string.Join(", ", header.Value)}");
                }

                return;
            }

            // For non-OPTIONS requests, continue with the pipeline
            // but set a flag to add CORS headers to the response
            context.Items["CorsMustAddHeaders"] = true;
            context.Items["CorsOrigin"] = origin;

            // Continue processing
            await next(context);

            // After the function executes, add CORS headers to the response
            if (context.GetInvocationResult()?.Value is HttpResponseData responseData)
            {
                Console.WriteLine("[CORS] Adding headers to response");

                string responseOrigin = context.Items.TryGetValue("CorsOrigin", out var originObj) ?
                    originObj as string : null;

                // Add CORS headers to the response
                if (!string.IsNullOrEmpty(responseOrigin))
                {
                    responseData.Headers.Add("Access-Control-Allow-Origin", responseOrigin);
                    responseData.Headers.Add("Access-Control-Allow-Credentials", "true");
                }
                else
                {
                    responseData.Headers.Add("Access-Control-Allow-Origin", "*");
                }

                // Log headers for debugging
                foreach (var header in responseData.Headers)
                {
                    Console.WriteLine($"[CORS] Response header: {header.Key}={string.Join(", ", header.Value)}");
                }
            }
            else
            {
                Console.WriteLine("[CORS] No HttpResponseData found in the result");
            }
        }
    }
}
