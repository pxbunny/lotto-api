namespace Lotto.Functions.Http.GetDrawResults;

[UsedImplicitly]
internal sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);

[UsedImplicitly]
internal sealed record DrawResultsCsvRecord(string DrawDate, string LottoNumbers, string? PlusNumbers);
