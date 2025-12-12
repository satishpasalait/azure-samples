# Azure Event Grid Samples

This folder contains comprehensive samples demonstrating Azure Event Grid functionality, including event publishing, subscription handling, and infrastructure deployment.

## Overview

Azure Event Grid is a fully managed event routing service that enables event-driven architectures. These samples demonstrate:

- **Event Publishing** - Publishing events to Event Grid topics using different schemas
- **Event Subscriptions** - Handling events via Azure Functions with Event Grid triggers
- **Event Filtering** - Advanced filtering using subject prefixes, event types, and custom attributes
- **Dead Letter Handling** - Configuring dead letter storage for failed events
- **Multiple Event Schemas** - Event Grid schema and CloudEvents schema support

## Project Structure

```
EventGrid/
├── Azure.Samples.EventGrid.Publisher/     # Console app for publishing events
│   ├── EventGridPublisher.cs              # Publisher class with multiple methods
│   ├── Models/                            # Event data models
│   │   ├── OrderEvent.cs
│   │   ├── UserEvent.cs
│   │   └── ProductEvent.cs
│   └── Program.cs                         # Main program with examples
├── Azure.Samples.EventGrid.Subscriber/    # Azure Function for handling events
│   ├── EventGridOrderHandler.cs           # Order event handler
│   ├── EventGridUserHandler.cs            # User event handler
│   ├── EventGridProductHandler.cs         # Product event handler
│   ├── EventGridCloudEventHandler.cs      # CloudEvent handler
│   └── Models/                            # Event data models
└── Infra/
    ├── main.bicep                         # Bicep template for Event Grid resources
    └── main.parameters.json               # Deployment parameters
```

## Features Demonstrated

### Publisher Features

1. **Custom Event Publishing** - Publish events using Event Grid schema
2. **Batch Publishing** - Send multiple events in a single batch
3. **CloudEvents Support** - Publish events using CloudEvents schema
4. **Custom Metadata** - Add extension attributes for filtering
5. **Multiple Event Types** - Order, User, and Product events
6. **Event Filtering Attributes** - Add custom attributes for advanced filtering

### Subscriber Features

1. **Event Grid Trigger** - Azure Functions with Event Grid triggers
2. **Multiple Handlers** - Separate handlers for different event types
3. **Event Deserialization** - Deserialize event data to strongly-typed models
4. **Error Handling** - Exception handling with retry and dead letter support
5. **CloudEvent Support** - Handle CloudEvents schema events
6. **Extension Attributes** - Access custom metadata from events

### Infrastructure Features

1. **Event Grid Topic** - Custom topic creation
2. **Event Subscriptions** - Multiple subscriptions with different filters
3. **Advanced Filtering** - Subject-based and advanced filters
4. **Dead Letter Storage** - Storage account for failed events
5. **Retry Policies** - Configurable retry and TTL settings
6. **Event Grid Domain** - Optional domain for advanced scenarios

## Prerequisites

- .NET SDK 10.0 (for Publisher)
- .NET SDK 8.0 (for Subscriber/Azure Functions)
- Azure Functions Core Tools
- Azure CLI
- An active Azure subscription

## Getting Started

### 1. Deploy Infrastructure

Deploy the Event Grid resources using the Bicep template:

```bash
cd EventGrid/Infra

# Login to Azure
az login

# Create resource group (if needed)
az group create --name rg-eventgrid-samples --location eastus

# Deploy the template
az deployment group create \
  --resource-group rg-eventgrid-samples \
  --template-file main.bicep \
  --parameters @main.parameters.json
```

After deployment, note the following from the outputs:
- Event Grid Topic Endpoint
- Event Grid Topic Access Key (retrieve from Azure Portal)

### 2. Configure Publisher

Set environment variables for the publisher:

```bash
# Windows PowerShell
$env:EVENT_GRID_TOPIC_ENDPOINT="https://your-topic-name.region.eventgrid.azure.net/api/events"
$env:EVENT_GRID_TOPIC_KEY="your-access-key"

# Linux/Mac
export EVENT_GRID_TOPIC_ENDPOINT="https://your-topic-name.region.eventgrid.azure.net/api/events"
export EVENT_GRID_TOPIC_KEY="your-access-key"
```

### 3. Run the Publisher

```bash
cd EventGrid/Azure.Samples.EventGrid.Publisher
dotnet restore
dotnet run
```

The publisher will send various types of events:
- Order events (Created, Processing, Shipped)
- User events (Created, Updated, Deleted)
- Product events (Created, Updated, Deleted, StockLow)
- Batch events
- CloudEvents

### 4. Configure and Deploy Subscriber

Update the Bicep template with your Azure Function resource ID, then deploy the subscriber:

```bash
cd EventGrid/Azure.Samples.EventGrid.Subscriber

# For local development
func start

# For Azure deployment
func azure functionapp publish <your-function-app-name>
```

## Event Types and Schemas

### Event Grid Schema

Events published using Event Grid schema include:
- `id` - Unique event identifier
- `subject` - Event subject (e.g., "orders/1001")
- `eventType` - Event type (e.g., "Order.Created")
- `eventTime` - Event timestamp
- `data` - Event payload
- `dataVersion` - Data schema version

### CloudEvents Schema

CloudEvents follow the CloudEvents 1.0 specification with:
- `id` - Unique event identifier
- `source` - Event source URI
- `type` - Event type
- `subject` - Event subject
- `time` - Event timestamp
- `data` - Event payload

## Event Filtering

The samples demonstrate several filtering mechanisms:

### Subject-Based Filtering

```csharp
// Filter events by subject prefix
subjectBeginsWith: "orders/"
```

### Event Type Filtering

```csharp
// Filter by specific event types
includedEventTypes: [
  "Order.Created",
  "Order.Processing"
]
```

### Advanced Filtering

```csharp
// Filter using event data properties
advancedFilters: [
  {
    operatorType: "StringContains",
    key: "data.category",
    values: ["Electronics", "Accessories"]
  }
]
```

### Extension Attributes

```csharp
// Add custom attributes for filtering
customEvent.ExtensionAttributes["priority"] = "high";
customEvent.ExtensionAttributes["region"] = "US-East";
```

## Dead Letter Handling

Failed events are automatically sent to dead letter storage when:
- Maximum delivery attempts are exceeded
- Event TTL expires
- Processing errors occur

Configure dead letter storage in the Bicep template by providing a storage account name.

## Retry Policies

Event Grid retry policies can be configured with:
- **Max Delivery Attempts** - Maximum retry count (default: 30)
- **Event TTL** - Time-to-live in minutes (default: 1440 = 24 hours)

## Best Practices

1. **Use Specific Event Types** - Use descriptive event types (e.g., "Order.Created" instead of "Event")
2. **Subject Naming** - Use hierarchical subjects (e.g., "orders/1001", "users/user-123")
3. **Idempotency** - Design handlers to be idempotent
4. **Error Handling** - Implement proper error handling and logging
5. **Dead Letter Monitoring** - Monitor dead letter storage for failed events
6. **Event Versioning** - Use `dataVersion` for schema evolution
7. **Filtering** - Use filters to reduce unnecessary event processing

## Additional Resources

- [Azure Event Grid Documentation](https://learn.microsoft.com/azure/event-grid/)
- [Event Grid Event Schema](https://learn.microsoft.com/azure/event-grid/event-schema)
- [CloudEvents Specification](https://cloudevents.io/)
- [Event Grid Filters](https://learn.microsoft.com/azure/event-grid/event-filtering)
- [Event Grid Retry Policies](https://learn.microsoft.com/azure/event-grid/delivery-and-retry)

