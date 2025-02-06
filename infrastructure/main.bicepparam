using './main.bicep'

var appName = 'lotto-draw-history'

param appServicePlanName = 'asp-${appName}'
param functionAppName = 'func-${appName}'
param storageAccountName = replace('st${appName}', '-', '')
param keyVaultName = 'kv-${appName}'
param drawResultsTableName = 'LottoResults'
param lottoApiKeySecretName = 'LottoApiKey'
param lottoBaseUrl = 'https://developers.lotto.pl/api/'
param githubSpObjectId = '' // Set through GitHub Actions
param dataUpdateSchedule = '0 30 22 * * 2,4,6'
param timeZone = 'Central European Standard Time'
