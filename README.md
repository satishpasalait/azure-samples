# Azure Samples

This repository contains sample code demonstrating various Azure services and patterns, including Azure Functions and Azure Service Bus messaging.

## Solution Overview

This solution provides practical examples for:
- **Azure Functions** - Serverless compute examples with HTTP triggers
- **Azure Service Bus** - Messaging patterns with producer/consumer examples
- **Infrastructure as Code** - Bicep templates for deploying Azure resources

## Solution Structure

```
azure-samples/
├── Functions/
│   └── Azure.Samples.Functions/          # Azure Functions project (.NET 8.0)
│       ├── AzureFunctionHelloWorld.cs    # HTTP trigger example
│       ├── AzureFunctionPostMessage.cs   # POST endpoint example
│       ├── AzureFunctionsGetGreetings.cs # GET endpoint example
│       └── Program.cs                    # Function app configuration
├── ServiceBus/
│   ├── Azure.Samples.ServiceBus.Producer/ # Service Bus message producer (.NET 10.0)
│   │   ├── ProducerServiceBus.cs
│   │   ├── Models/Order.cs
│   │   └── Program.cs
│   ├── Azure.Samples.ServiceBus.Consumer/  # Service Bus message consumer (.NET 10.0)
│   │   ├── ConsumerServiceBus.cs
│   │   ├── Models/Order.cs
│   │   └── Program.cs
│   └── Infra/
│       ├── main.bicep                     # Bicep template for Service Bus resources
│       └── main.parameters.json           # Deployment parameters
└── Azure.Samples.sln                      # Visual Studio solution file
```

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) or later (for Functions)
- [.NET SDK 10.0](https://dotnet.microsoft.com/download) or later (for Service Bus projects)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) (for local development)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) (for deploying infrastructure)
- An active Azure subscription
- Visual Studio 2022 or Visual Studio Code (recommended)

## Azure Functions Samples

The Functions project demonstrates serverless compute patterns using Azure Functions v4 with the isolated worker process model.

### Features

- **HTTP Triggers** - Examples of GET and POST endpoints
- **Application Insights Integration** - Telemetry and monitoring configured
- **Dependency Injection** - Modern .NET patterns with DI support

### Available Functions

- `HelloWorld` - HTTP trigger that accepts GET/POST requests with optional `name` query parameter
- Additional functions available in the project

### Running Locally

1. Navigate to the Functions project:
   ```bash
   cd Functions/Azure.Samples.Functions
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the function app:
   ```bash
   func start
   ```

4. The functions will be available at:
   - `http://localhost:7071/api/hello?name=YourName`

### Configuration

The Functions app uses `host.json` for configuration. Application Insights is configured in `Program.cs`.

## Azure Service Bus Samples

The Service Bus samples demonstrate message producer and consumer patterns using Azure Service Bus queues.

### Producer Application

The producer application sends order messages to a Service Bus queue.

**Configuration** (via environment variables or `appsettings.json`):
- `SERVICE_BUS_CONNECTION_STRING` - Connection string to your Service Bus namespace
- `SERVICE_BUS_QUEUE_NAME` - Name of the queue (default: "orders")

**Running the Producer:**

1. Navigate to the producer project:
   ```bash
   cd ServiceBus/Azure.Samples.ServiceBus.Producer
   ```

2. Set environment variables:
   ```bash
   # Windows PowerShell
   $env:SERVICE_BUS_CONNECTION_STRING="your-connection-string"
   $env:SERVICE_BUS_QUEUE_NAME="orders"

   # Linux/Mac
   export SERVICE_BUS_CONNECTION_STRING="your-connection-string"
   export SERVICE_BUS_QUEUE_NAME="orders"
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

The producer will send 10 sample order messages to the queue.

### Consumer Application

The consumer application receives and processes messages from a Service Bus queue.

**Configuration** (via environment variables or `appsettings.json`):
- `SERVICE_BUS_CONNECTION_STRING` - Connection string to your Service Bus namespace
- `SERVICE_BUS_QUEUE_NAME` - Name of the queue (default: "orders")

**Running the Consumer:**

1. Navigate to the consumer project:
   ```bash
   cd ServiceBus/Azure.Samples.ServiceBus.Consumer
   ```

2. Set environment variables (same as producer)

3. Run the application:
   ```bash
   dotnet run
   ```

The consumer will continuously listen for messages and process them as they arrive.

## Infrastructure as Code

The `ServiceBus/Infra` folder contains Bicep templates for deploying Azure Service Bus resources.

### Deploying Infrastructure

1. Navigate to the infrastructure folder:
   ```bash
   cd ServiceBus/Infra
   ```

2. Log in to Azure:
   ```bash
   az login
   ```

3. Create a resource group (if needed):
   ```bash
   az group create --name rg-azure-samples --location eastus
   ```

4. Deploy the Bicep template:
   ```bash
   az deployment group create \
     --resource-group rg-azure-samples \
     --template-file main.bicep \
     --parameters @main.parameters.json
   ```

### Bicep Template Resources

The template creates:
- **Service Bus Namespace** - Messaging namespace (Basic, Standard, or Premium tier)
- **Service Bus Queue** - Queue for point-to-point messaging
- **Service Bus Topic** - Topic for publish-subscribe messaging
- **Topic Subscription** - Subscription for the topic

### Parameters

Edit `main.parameters.json` to customize:
- `serviceBusNamespaceName` - Unique namespace name
- `skuName` - SKU tier (Basic, Standard, Premium)
- `queueName` - Queue name
- `topicName` - Topic name
- `subscriptionName` - Subscription name

## Getting Started

1. **Clone the repository** (if applicable)

2. **Deploy Infrastructure:**
   - Deploy the Service Bus resources using the Bicep template
   - Note the connection string from the Azure portal

3. **Configure Applications:**
   - Set environment variables for Service Bus connection strings
   - Update `appsettings.json` files if preferred

4. **Run the Samples:**
   - Start the Service Bus consumer first (to listen for messages)
   - Run the producer to send messages
   - Test the Azure Functions locally or deploy to Azure

## Technologies Used

### .NET 8.0
The Azure Functions project uses .NET 8.0, Microsoft's cross-platform framework for building cloud applications. This version provides improved performance, enhanced language features, and long-term support. It's the recommended runtime for Azure Functions v4, offering better isolation and compatibility with modern .NET patterns.

### .NET 10.0
The Service Bus producer and consumer applications leverage .NET 10.0, the latest .NET framework version. This provides access to the newest language features, performance improvements, and ensures compatibility with the latest Azure SDK packages. It enables modern C# features like top-level statements, nullable reference types, and async/await patterns.

### Azure Functions v4
Azure Functions is a serverless compute service that allows you to run event-driven code without managing infrastructure. Version 4 uses the isolated worker process model, which provides better control over the hosting environment, improved performance, and the ability to use any .NET version. This model runs your functions in a separate process from the Functions host, enabling more flexibility and better isolation.

### Azure Service Bus
Azure Service Bus is a fully managed enterprise message broker with message queues and publish-subscribe topics. It enables decoupled communication between applications and services, providing reliable message delivery, ordering guarantees, and support for complex messaging patterns. Service Bus supports both point-to-point (queues) and publish-subscribe (topics/subscriptions) messaging patterns, making it ideal for building scalable, distributed applications.

### Azure Bicep
Azure Bicep is a domain-specific language (DSL) for declaratively deploying Azure resources. It's a transparent abstraction over Azure Resource Manager (ARM) templates, providing a cleaner syntax, better type safety, and improved developer experience. Bicep simplifies infrastructure deployment by allowing you to define, version, and deploy Azure resources as code, ensuring consistency and repeatability across environments.

### Application Insights
Azure Application Insights is an extensible Application Performance Management (APM) service for developers and DevOps professionals. It provides automatic instrumentation, custom telemetry collection, and powerful analytics to monitor application performance, detect issues, and understand user behavior. In this solution, Application Insights is integrated with Azure Functions to provide real-time monitoring, performance metrics, and diagnostic information.

### Azure.Messaging.ServiceBus
The Azure.Messaging.ServiceBus NuGet package (version 7.20.1) provides the client library for interacting with Azure Service Bus. It offers a modern, async-first API for sending and receiving messages, managing sessions, and handling advanced messaging scenarios. The library supports both queue and topic/subscription patterns with built-in retry policies and connection management.

### Microsoft.Azure.Functions.Worker
The Azure Functions Worker SDK enables building Azure Functions using the isolated worker process model. It provides the core functionality for function execution, trigger bindings, and integration with the Azure Functions runtime. This SDK allows developers to use dependency injection, middleware, and other modern .NET patterns in their functions.

## Additional Resources

- [Azure Functions Documentation](https://learn.microsoft.com/azure/azure-functions/)
- [Azure Service Bus Documentation](https://learn.microsoft.com/azure/service-bus-messaging/)
- [Azure Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)

## License

This repository contains sample code for educational and demonstration purposes.
