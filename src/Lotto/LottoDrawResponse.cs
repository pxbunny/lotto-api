namespace Lotto.Lotto;

[UsedImplicitly]
internal sealed record LottoDrawResponse(int DrawSystemId, DateTime DrawDate, string GameType, IEnumerable<LottoDrawResults> Results);

[UsedImplicitly]
internal sealed record LottoDrawResults(IEnumerable<int> ResultsJson);
