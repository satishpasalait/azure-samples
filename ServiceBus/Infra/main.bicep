@description('Location for all resources.')
param location string = resourceGroup().location

@description('Service Bus namespace name.')
param serviceBusNamespaceName string

@allowed([
  'Basic'
  'Standard'
  'Premium'
])
@description('SKU /Tier for Service Bus')
param skuName string = 'Standard'

@description('Name of the Service Bus queue')
param queueName string = 'orders'

@description('Name of the Service Bus topic')
param topicName string = 'orders-topic'

@description('Name of the subscription for the topic')
param subscriptionName string = 'orders-subscription'

@description('Max delivery count before message is dead-lettered')
param maxDeliveryCount int = 10

@description('Enable partitioning')
param enablePartitioning bool = true

//--------------------------------
// Service Bus Namespace
//--------------------------------
resource sbNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: skuName
    tier: skuName
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

//--------------------------------
// Service Bus Queue
//--------------------------------
resource sbQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01' = {
  name: '${sbNamespace.name}/${queueName}'
  properties: {
    enablePartitioning: enablePartitioning
    maxDeliveryCount: maxDeliveryCount
    deadLetteringOnMessageExpiration: true
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}

//--------------------------------
// Service Bus Topic
//--------------------------------
resource sbTopic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01' = {
  name: '${sbNamespace.name}/${topicName}'
  properties: {
    enablePartitioning: enablePartitioning
    defaultMessageTimeToLive: 'P14D'
  }
}

//--------------------------------
// Service Bus Topic Subscription
//--------------------------------
resource sbSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01' = {
  name: '${sbNamespace.name}/${topicName}/${subscriptionName}'
  properties: {
    maxDeliveryCount: maxDeliveryCount
    deadLetteringOnMessageExpiration: true
    lockDuration: 'PT30S'
  }
}

//--------------------------------
// Outputs
//--------------------------------
output serviceBusNamespaceId string = sbNamespace.id
output serviceBusQueueId string = sbQueue.id
output serviceBusTopicId string = sbTopic.id
output serviceBusSubscriptionId string = sbSubscription.id
