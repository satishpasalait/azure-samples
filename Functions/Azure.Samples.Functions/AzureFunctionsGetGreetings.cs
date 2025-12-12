
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Azure.Samples.Functions
{
    public class AzureFunctionsGetGreetings
    {
        private readonly ILogger _logger;

        public AzureFunctionsGetGreetings(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AzureFunctionsGetGreetings>();
        }

        [Function("GetGreetings")]
        public async Task<HttpResponseData> GetGreetings(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "greeting")] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("GetGreetings function processed a request");

            var name = GetQueryParameters(req, "name") ?? "Guest";
            var greeting = new
            {
                message = $"Hello, {name}",
                timestamp = DateTime.UtcNow,
                method = req.Method
            };

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonSerializer.Serialize(greeting));

            return response;
        }

        private string? GetQueryParameters(HttpRequestData request, string parameterName)
        {
            if (request.FunctionContext.BindingContext.BindingData.TryGetValue(parameterName, out var value) && value is string stringValue)
            {
                return stringValue;
            }

            var queryString = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
            return queryString[parameterName];
        }

    }
}
