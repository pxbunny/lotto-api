namespace LottoDrawHistory.Lotto;

[UsedImplicitly]
sealed record LottoDrawResponse(int DrawSystemId, DateTime DrawDate, string GameType, IEnumerable<LottoDrawResults> Results);

[UsedImplicitly]
sealed record LottoDrawResults(IEnumerable<int> ResultsJson);
