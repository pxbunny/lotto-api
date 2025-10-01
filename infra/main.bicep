//*************************************
// Parameters
//*************************************

@description('A unique token used for resource name generation.')
@minLength(3)
param resourceToken string = toLower(uniqueString(subscription().id, location))

@description('A unique token used for resource name generation without dashes.')
@minLength(1)
param resourceTokenWithoutDashes string = replace(resourceToken, '-', '')

@description('Name of the table for storing draw results.')
param drawResultsTableName string

@description('Name of the secret in Key Vault storing Lotto API key.')
param lottoApiKeySecretName string

@description('Base URL for the original Lotto API.')
param lottoBaseUrl string

@description('GitHub Service Principal Object ID.')
param githubSpObjectId string

@description('Data sync schedule in cron format.')
param dataUpdateSchedule string

@description('Time zone for scheduling data sync.')
param timeZone string

@description('Primary region for all Azure resources.')
@minLength(1)
param location string = resourceGroup().location


//*************************************
// Resources
//*************************************

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2025-02-01' = {
  name: 'log-${resourceToken}'
  location: location
  properties: any({
    retentionInDays: 7
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-${resourceToken}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: 'st${resourceTokenWithoutDashes}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }

  resource tableServices 'tableServices' = {
    name: 'default'

    resource drawResultsTable 'tables' = {
      name: drawResultsTableName
    }
  }
}

resource storageDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'storage-account-logs'
  scope: storageAccount
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        category: 'StorageRead'
        enabled: true
      }
      {
        category: 'StorageWrite'
        enabled: true
      }
      {
        category: 'StorageDelete'
        enabled: true
      }
      {
        category: 'StorageTableLogs'
        enabled: true
      }
      {
        category: 'StorageBlobLogs'
        enabled: true
      }
      {
        category: 'Transaction'
        enabled: true
      }
    ]
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: 'asp-${resourceToken}'
  kind: 'functionapp'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2024-11-01' = {
  name: 'kv-${resourceToken}'
  location: location
  properties: {
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
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
  name: 'func-${resourceToken}'
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
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: 'InstrumentationKey=${appInsights.properties.InstrumentationKey}'
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
          value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${lottoApiKeySecretName})'
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


//*************************************
// Outputs
//*************************************

output appName string = functionApp.name
output keyVaultName string = keyVault.name
output secretName string = lottoApiKeySecretName
