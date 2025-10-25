namespace Lotto.Features.GetDrawResults.FunctionHelpers;

sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);

sealed record DrawResultsCsvRecord(string DrawDate, string LottoNumbers, string? PlusNumbers);
