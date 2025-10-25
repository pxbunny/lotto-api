namespace Lotto.LottoClient;

sealed record LottoDrawResultsResponse(int DrawSystemId, DateTime DrawDate, string GameType, IEnumerable<LottoDrawResultsItem> Results);

sealed record LottoDrawResultsItem(IEnumerable<int> ResultsJson);
