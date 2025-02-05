param appServicePlanName string
param functionAppName string
param storageAccountName string
param drawResultsTableName string

param location string = resourceGroup().location

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-05-01' = {
  name: 'default'
  parent: storageAccount
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  kind: 'functionapp'
  location: location
  sku: {
    name: 'Y1'
  }
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  kind: 'functionapp'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
  }
}

resource drawResultsTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-05-01' = {
  name: drawResultsTableName
  parent: tableService
}

output appName string = functionApp.name
