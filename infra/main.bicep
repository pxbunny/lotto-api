param appServicePlanName string
param functionAppName string
param storageAccountName string
param drawResultsTableName string
param keyVaultName string
param lottoApiKeySecretName string
param lottoBaseUrl string
param githubSpObjectId string
param dataUpdateSchedule string
param timeZone string

param location string = resourceGroup().location

resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }

  resource tableServices 'tableServices' = {
    name: 'default'

    resource drawResultsTable 'tables' = {
      name: drawResultsTableName
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: appServicePlanName
  kind: 'functionapp'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2024-11-01' = {
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

  resource lottoApiKeySecret 'secrets' = {
    name: lottoApiKeySecretName
    properties: {
      value: 'PLACEHOLDER' // Set through GitHub Actions
    }
  }
}

resource functionApp 'Microsoft.Web/sites@2024-11-01' = {
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
          name: 'WEBSITE_TIME_ZONE'
          value: timeZone
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
          value: lottoBaseUrl
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

resource accessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2024-11-01' = {
  name: 'add'
  parent: keyVault
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
