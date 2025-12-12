@description('Location for all resources.')
param location string = resourceGroup().location

@description('Event Grid topic name.')
param topicName string

@description('Event Grid domain name (optional, for domain-based topics).')
param domainName string = ''

@description('Enable input schema validation')
param enableInputSchemaValidation bool = true

@description('Maximum delivery count for event subscriptions')
param maxDeliveryCount int = 30

@description('Dead letter storage account name (optional)')
param deadLetterStorageAccountName string = ''

@description('Dead letter storage container name')
param deadLetterContainerName string = 'deadletter'

@description('Event subscription name for orders')
param orderSubscriptionName string = 'orders-subscription'

@description('Event subscription name for users')
param userSubscriptionName string = 'users-subscription'

@description('Event subscription name for products')
param productSubscriptionName string = 'products-subscription'

@description('Event subscription name for cloud events')
param cloudEventSubscriptionName string = 'cloudevents-subscription'

//--------------------------------
// Storage Account for Dead Letter (if provided)
//--------------------------------
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = if (!empty(deadLetterStorageAccountName))
{
  name: deadLetterStorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource storageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = if (!empty(deadLetterStorageAccountName))
{
  name: '${storageAccount.name}/default/${deadLetterContainerName}'
  properties: {
    publicAccess: 'None'
  }
}

//--------------------------------
// Event Grid Topic
//--------------------------------
resource eventGridTopic 'Microsoft.EventGrid/topics@2022-06-15' = {
  name: topicName
  location: location
  sku: {
    name: 'Basic'
  }
  kind: 'Azure'
  properties: {
    inputSchema: 'EventGridSchema'
    inputSchemaMapping: null
    publicNetworkAccess: 'Enabled'
    inboundIpRules: []
    disableLocalAuth: false
    dataResidencyBoundary: 'WithinGeopair'
  }
}

//--------------------------------
// Event Grid Domain (optional, for advanced scenarios)
//--------------------------------
resource eventGridDomain 'Microsoft.EventGrid/domains@2022-06-15' = if (!empty(domainName))
{
  name: domainName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    inputSchema: 'EventGridSchema'
    publicNetworkAccess: 'Enabled'
    inboundIpRules: []
    disableLocalAuth: false
    dataResidencyBoundary: 'WithinGeopair'
  }
}

//--------------------------------
// Event Subscription for Orders
//--------------------------------
resource orderEventSubscription 'Microsoft.EventGrid/topics/eventSubscriptions@2022-06-15' = {
  parent: eventGridTopic
  name: orderSubscriptionName
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '' // Set this to your Azure Function resource ID
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
      }
    }
    filter: {
      includedEventTypes: [
        'Order.Created'
        'Order.Processing'
        'Order.Shipped'
        'Order.Delivered'
        'Order.Cancelled'
      ]
      subjectBeginsWith: 'orders/'
      subjectEndsWith: ''
      isSubjectCaseSensitive: false
      advancedFilters: []
    }
    labels: []
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: maxDeliveryCount
      eventTimeToLiveInMinutes: 1440
    }
    deadLetterDestination: !empty(deadLetterStorageAccountName) ? {
      endpointType: 'StorageBlob'
      properties: {
        blobContainerName: deadLetterContainerName
        resourceId: storageAccount.id
      }
    } : null
    deadLetterWithResourceIdentity: null
  }
}

//--------------------------------
// Event Subscription for Users
//--------------------------------
resource userEventSubscription 'Microsoft.EventGrid/topics/eventSubscriptions@2022-06-15' = {
  parent: eventGridTopic
  name: userSubscriptionName
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '' // Set this to your Azure Function resource ID
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
      }
    }
    filter: {
      includedEventTypes: [
        'User.Created'
        'User.Updated'
        'User.Deleted'
      ]
      subjectBeginsWith: 'users/'
      subjectEndsWith: ''
      isSubjectCaseSensitive: false
      advancedFilters: []
    }
    labels: []
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: maxDeliveryCount
      eventTimeToLiveInMinutes: 1440
    }
    deadLetterDestination: !empty(deadLetterStorageAccountName) ? {
      endpointType: 'StorageBlob'
      properties: {
        blobContainerName: deadLetterContainerName
        resourceId: storageAccount.id
      }
    } : null
  }
}

//--------------------------------
// Event Subscription for Products with Advanced Filtering
//--------------------------------
resource productEventSubscription 'Microsoft.EventGrid/topics/eventSubscriptions@2022-06-15' = {
  parent: eventGridTopic
  name: productSubscriptionName
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '' // Set this to your Azure Function resource ID
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
      }
    }
    filter: {
      includedEventTypes: [
        'ProductCreated'
        'ProductUpdated'
        'ProductDeleted'
        'StockLow'
      ]
      subjectBeginsWith: 'products/'
      subjectEndsWith: ''
      isSubjectCaseSensitive: false
      advancedFilters: [
        {
          operatorType: 'StringContains'
          key: 'data.category'
          values: [
            'Electronics'
            'Accessories'
          ]
        }
      ]
    }
    labels: []
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: maxDeliveryCount
      eventTimeToLiveInMinutes: 1440
    }
    deadLetterDestination: !empty(deadLetterStorageAccountName) ? {
      endpointType: 'StorageBlob'
      properties: {
        blobContainerName: deadLetterContainerName
        resourceId: storageAccount.id
      }
    } : null
  }
}

//--------------------------------
// Event Subscription for CloudEvents
//--------------------------------
resource cloudEventSubscription 'Microsoft.EventGrid/topics/eventSubscriptions@2022-06-15' = {
  parent: eventGridTopic
  name: cloudEventSubscriptionName
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '' // Set this to your Azure Function resource ID
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
      }
    }
    filter: {
      includedEventTypes: [
        'com.example.CloudEvent'
      ]
      subjectBeginsWith: 'cloud-events/'
      subjectEndsWith: ''
      isSubjectCaseSensitive: false
      advancedFilters: []
    }
    labels: []
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    retryPolicy: {
      maxDeliveryAttempts: maxDeliveryCount
      eventTimeToLiveInMinutes: 1440
    }
    deadLetterDestination: !empty(deadLetterStorageAccountName) ? {
      endpointType: 'StorageBlob'
      properties: {
        blobContainerName: deadLetterContainerName
        resourceId: storageAccount.id
      }
    } : null
  }
}

//--------------------------------
// Outputs
//--------------------------------
output eventGridTopicEndpoint string = eventGridTopic.properties.endpoint
output eventGridTopicName string = eventGridTopic.name
output eventGridTopicResourceId string = eventGridTopic.id
output eventGridDomainEndpoint string = !empty(domainName) ? eventGridDomain.properties.endpoint : ''
output eventGridDomainName string = !empty(domainName) ? eventGridDomain.name : ''
output storageAccountName string = !empty(deadLetterStorageAccountName) ? storageAccount.name : ''

