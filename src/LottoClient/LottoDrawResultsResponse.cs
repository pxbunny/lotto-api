namespace Lotto.LottoClient;

internal sealed record LottoDrawResultsResponse(int DrawSystemId, DateTime DrawDate, string GameType, IEnumerable<LottoDrawResultsItem> Results);

internal sealed record LottoDrawResultsItem(IEnumerable<int> ResultsJson);
