using './main.bicep'

var appName = 'lotto-draw-history'

param appServicePlanName = 'asp-${appName}'
param functionAppName = 'func-${appName}'
param storageAccountName = replace('st${appName}', '-', '')
param keyVaultName = 'kv-${appName}'
param drawResultsTableName = 'LottoResults'
param lottoApiKeySecretName = 'LottoApiKey'
param githubSpObjectId = '' // Set through GitHub Actions
param dataUpdateSchedule = '0 30 21 * * 2,4,6'
