//using System.Net;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.Logging;
//using System.Text.Json;
//using System.Web;
//using Microsoft.Azure.Functions.Worker.Extensions.Timer;

//namespace Azure.Samples.Functions;

//public class SampleFunctions
//{
//    private readonly ILogger _logger;

//    public SampleFunctions(ILoggerFactory loggerFactory)
//    {
//        _logger = loggerFactory.CreateLogger<SampleFunctions>();
//    }

//    [Function("HelloWorld")]
//    public HttpResponseData HelloWorld(
//        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "hello")] HttpRequestData req,
//        FunctionContext executionContext)
//    {
//        _logger.LogInformation("HelloWorld function processed a request.");

//        var response = req.CreateResponse(HttpStatusCode.OK);
//        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        
//        var name = GetQueryParameter(req, "name") ?? "World";
//        response.WriteString($"Hello, {name}!");

//        return response;
//    }

//    [Function("GetGreeting")]
//    public HttpResponseData GetGreeting(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "greeting")] HttpRequestData req)
//    {
//        _logger.LogInformation("GetGreeting function processed a request.");

//        var name = GetQueryParameter(req, "name") ?? "Guest";
//        var greeting = new
//        {
//            message = $"Hello, {name}!",
//            timestamp = DateTime.UtcNow,
//            method = req.Method
//        };

//        var response = req.CreateResponse(HttpStatusCode.OK);
//        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
//        response.WriteString(JsonSerializer.Serialize(greeting));

//        return response;
//    }

//    [Function("PostMessage")]
//    public async Task<HttpResponseData> PostMessage(
//        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "message")] HttpRequestData req)
//    {
//        _logger.LogInformation("PostMessage function processed a request.");

//        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        
//        if (string.IsNullOrEmpty(requestBody))
//        {
//            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
//            errorResponse.WriteString("Request body is required.");
//            return errorResponse;
//        }

//        try
//        {
//            var message = JsonSerializer.Deserialize<MessageRequest>(requestBody, new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            });

//            var response = req.CreateResponse(HttpStatusCode.OK);
//            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
//            var responseBody = new
//            {
//                success = true,
//                receivedMessage = message?.Message ?? "No message provided",
//                sender = message?.Sender ?? "Unknown",
//                timestamp = DateTime.UtcNow
//            };

//            response.WriteString(JsonSerializer.Serialize(responseBody));
//            return response;
//        }
//        catch (JsonException ex)
//        {
//            _logger.LogError(ex, "Error parsing request body.");
//            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
//            errorResponse.WriteString($"Invalid JSON: {ex.Message}");
//            return errorResponse;
//        }
//    }

//    /// <summary>
//    /// Timer trigger function that runs on a schedule
//    /// Runs every 5 minutes using cron expression: "0 */5 * * * *"
//    /// </summary>
//    [Function("ScheduledTask")]
//    public void ScheduledTask(
//        [TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
//    {
//        _logger.LogInformation($"Scheduled task executed at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
//        _logger.LogInformation($"Next execution time: {timerInfo.ScheduleStatus?.Next:yyyy-MM-dd HH:mm:ss} UTC");
        
//        // Example: Perform scheduled tasks like cleanup, data processing, etc.
//        _logger.LogInformation("Processing scheduled maintenance tasks...");
//    }

//    /// <summary>
//    /// Health check endpoint to monitor function app status
//    /// </summary>
//    [Function("HealthCheck")]
//    public HttpResponseData HealthCheck(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
//    {
//        _logger.LogInformation("Health check endpoint accessed.");

//        var healthStatus = new
//        {
//            status = "healthy",
//            timestamp = DateTime.UtcNow,
//            version = "1.0.0",
//            environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ?? "Development"
//        };

//        var response = req.CreateResponse(HttpStatusCode.OK);
//        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
//        response.WriteString(JsonSerializer.Serialize(healthStatus));

//        return response;
//    }

//    /// <summary>
//    /// Calculator function that performs basic math operations
//    /// Example: GET /api/calculate?operation=add&a=5&b=3
//    /// </summary>
//    [Function("Calculate")]
//    public HttpResponseData Calculate(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "calculate")] HttpRequestData req)
//    {
//        _logger.LogInformation("Calculate function processed a request.");

//        var operation = GetQueryParameter(req, "operation")?.ToLower() ?? "add";
//        var aStr = GetQueryParameter(req, "a");
//        var bStr = GetQueryParameter(req, "b");

//        if (string.IsNullOrEmpty(aStr) || string.IsNullOrEmpty(bStr))
//        {
//            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
//            errorResponse.WriteString("Parameters 'a' and 'b' are required.");
//            return errorResponse;
//        }

//        if (!double.TryParse(aStr, out var a) || !double.TryParse(bStr, out var b))
//        {
//            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
//            errorResponse.WriteString("Parameters 'a' and 'b' must be valid numbers.");
//            return errorResponse;
//        }

//        try
//        {
//            double result = operation switch
//            {
//                "add" => a + b,
//                "subtract" => a - b,
//                "multiply" => a * b,
//                "divide" => b != 0 ? a / b : throw new DivideByZeroException("Cannot divide by zero"),
//                _ => throw new ArgumentException($"Unknown operation: {operation}. Supported operations: add, subtract, multiply, divide")
//            };

//            var response = req.CreateResponse(HttpStatusCode.OK);
//            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
//            var responseBody = new
//            {
//                operation = operation,
//                operand1 = a,
//                operand2 = b,
//                result = result,
//                timestamp = DateTime.UtcNow
//            };

//            response.WriteString(JsonSerializer.Serialize(responseBody));
//            return response;
//        }
//        catch (DivideByZeroException ex)
//        {
//            _logger.LogWarning(ex, "Division by zero attempted.");
//            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
//            errorResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
//            errorResponse.WriteString(JsonSerializer.Serialize(new { error = ex.Message }));
//            return errorResponse;
//        }
//        catch (ArgumentException ex)
//        {
//            _logger.LogWarning(ex, "Invalid operation requested.");
//            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
//            errorResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
//            errorResponse.WriteString(JsonSerializer.Serialize(new { error = ex.Message }));
//            return errorResponse;
//        }
//    }

//    /// <summary>
//    /// Function that demonstrates path parameters
//    /// Example: GET /api/users/123
//    /// </summary>
//    [Function("GetUser")]
//    public HttpResponseData GetUser(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id:int}")] HttpRequestData req,
//        int id)
//    {
//        _logger.LogInformation($"GetUser function processed a request for user ID: {id}");

//        // Simulate user lookup
//        var user = new
//        {
//            id = id,
//            name = $"User{id}",
//            email = $"user{id}@example.com",
//            createdDate = DateTime.UtcNow.AddDays(-id),
//            status = "active"
//        };

//        var response = req.CreateResponse(HttpStatusCode.OK);
//        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
//        response.WriteString(JsonSerializer.Serialize(user));

//        return response;
//    }

//    /// <summary>
//    /// Function that demonstrates multiple HTTP methods and custom headers
//    /// </summary>
//    [Function("EchoRequest")]
//    public async Task<HttpResponseData> EchoRequest(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "echo")] HttpRequestData req)
//    {
//        _logger.LogInformation($"EchoRequest function processed a {req.Method} request.");

//        var requestBody = string.Empty;
//        if (req.Body != null && req.Body.CanRead)
//        {
//            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//        }

//        var headers = new Dictionary<string, string>();
//        foreach (var header in req.Headers)
//        {
//            headers[header.Key] = string.Join(", ", header.Value);
//        }

//        var echoResponse = new
//        {
//            method = req.Method,
//            url = req.Url.ToString(),
//            query = req.Url.Query,
//            headers = headers,
//            body = requestBody,
//            timestamp = DateTime.UtcNow
//        };

//        var response = req.CreateResponse(HttpStatusCode.OK);
//        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
//        response.WriteString(JsonSerializer.Serialize(echoResponse, new JsonSerializerOptions { WriteIndented = true }));

//        return response;
//    }

//    private string? GetQueryParameter(HttpRequestData request, string parameterName)
//    {
//        if (request.FunctionContext.BindingContext.BindingData.TryGetValue(parameterName, out var value) && value is string stringValue)
//        {
//            return stringValue;
//        }
        
//        var queryString = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
//        return queryString[parameterName];
//    }
//}

//public class MessageRequest
//{
//    public string? Message { get; set; }
//    public string? Sender { get; set; }
//}
