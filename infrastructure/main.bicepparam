using './main.bicep'

param appServicePlanName = 'asp-lotto-draw-history'
param functionAppName = 'func-lotto-draw-history'
param storageAccountName = toLower('stLottoDrawHistory')
param drawResultsTableName = 'DrawResults'
param errorsTableName = 'Errors'
