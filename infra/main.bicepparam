using './main.bicep'

param resourceToken = 'lotto-api-test1'
param lottoApiKeySecretName = 'LottoApiKey'
param lottoBaseUrl = 'https://developers.lotto.pl/api/'
param githubSpObjectId = '' // Set through GitHub Actions
param dataSyncSchedule = '0 45 22 * * 2,4,6'
param timeZone = 'Central European Standard Time'

param drawResultsTableName = 'LottoDrawResults'

param disabledFunctions = ['CreateDrawResultsTable', 'DropDrawResultsTable']
