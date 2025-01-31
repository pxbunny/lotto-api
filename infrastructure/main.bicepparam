using './main.bicep'

var appName = 'lotto-draw-history'

param appServicePlanName = 'asp-${appName}'
param functionAppName = 'func-${appName}'
param storageAccountName = replace('st${appName}', '-', '')
param drawResultsTableName = 'DrawResults'
param errorsTableName = 'Errors'
