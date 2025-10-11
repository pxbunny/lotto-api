namespace Lotto.LottoClient;

[UsedImplicitly]
internal sealed record LottoDrawResultsResponse(int DrawSystemId, DateTime DrawDate, string GameType, IEnumerable<LottoDrawResultsItem> Results);

[UsedImplicitly]
internal sealed record LottoDrawResultsItem(IEnumerable<int> ResultsJson);
