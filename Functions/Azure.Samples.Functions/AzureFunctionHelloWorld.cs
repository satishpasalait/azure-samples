using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Azure.Samples.Functions
{
    public class AzureFunctionHelloWorld
    {
        private readonly ILogger _logger;

        public AzureFunctionHelloWorld(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AzureFunctionHelloWorld>();
        }

        [Function("HelloWorld")]
        public async Task<HttpResponseData> HelloWorld(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "hello")] HttpRequestData req,
            FunctionContext executeContext)
        
        {
            _logger.LogInformation("HelloWorld function processed a request.");

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var name = GetQueryParameters(req, "name") ?? "World";
            await response.WriteStringAsync($"Hello, {name}!");

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
