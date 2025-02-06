param appServicePlanName string
param functionAppName string
param storageAccountName string
param drawResultsTableName string
param keyVaultName string
param lottoApiKeySecretName string
param githubSpObjectId string
param dataUpdateSchedule string

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

resource drawResultsTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-05-01' = {
  name: drawResultsTableName
  parent: tableService
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  kind: 'functionapp'
  location: location
  sku: {
    name: 'Y1'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: false
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: githubSpObjectId
        permissions: {
          secrets: ['set', 'get', 'list']
        }
      }
    ]
  }
}

resource lottoApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: lottoApiKeySecretName
  parent: keyVault
  properties: {
    value: 'PLACEHOLDER' // Set through GitHub Actions
  }
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  kind: 'functionapp'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
        }
        {
          name: 'DataUpdateSchedule'
          value: dataUpdateSchedule
        }
        {
          name: 'LottoBaseUrl'
          value: 'https://developers.lotto.pl/api/'
        }
        {
          name: 'LottoApiKey'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/${lottoApiKeySecretName}/)'
        }
      ]
    }
    httpsOnly: true
  }
}

resource accessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: functionApp.identity.principalId
        permissions: {
          secrets: ['get']
        }
      }
    ]
  }
}

output appName string = functionApp.name
output keyVaultName string = keyVault.name
output secretName string = lottoApiKeySecretName
